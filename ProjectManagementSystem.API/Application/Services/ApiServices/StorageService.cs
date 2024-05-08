﻿using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Domain.Models.InternalSerives.Storage.Request;
using Domain.Models.ServiceResponses.Storage;
using Domain.Services.InternalServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Application.Services.ApiServices
{
    public class StorageService(IConfiguration configuration,
       ILogger<StorageService> logger) : IStorageService
    {
        private static IAmazonS3 _s3Client;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<StorageService> _logger = logger;

        public async Task<UploadFileServiceResponse> Upload(UploadFileRequest request, string userId)
        {
            try
            {
                var awsCredentials = new Amazon.Runtime
                .BasicAWSCredentials(
                    _configuration["AwsS3:AccessKey"],
                    _configuration["AwsS3:SecretKey"]);

                var config = new AmazonS3Config
                { ServiceURL = _configuration["AwsS3:UrlAdress"] };

                _s3Client = new AmazonS3Client
                    (awsCredentials, config);

                using (MemoryStream stream = new())
                {
                    request.File.CopyTo(stream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = stream,
                        Key = request.Key,
                        BucketName = _configuration["AwsS3:BucketName"],
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(_s3Client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }

                return new UploadFileServiceResponse(
                     Domain.Models.ServiceResponses.Base.ServiceResponseStatusBase.Success);
            }
            catch (Exception ex)
            {
                _logger.LogError("UploadFileService : {Message}", ex.Message);

                return new UploadFileServiceResponse(
                     Domain.Models.ServiceResponses.Base.ServiceResponseStatusBase.InternalError);
            }
        }

        public async Task<GetFileUrlServiceResponse> GetUrl(GetFileUrlRequest request)
        {
            try
            {
                var awsCredentials = new Amazon.Runtime
                   .BasicAWSCredentials(
                   _configuration["AwsS3:AccessKey"],
                   _configuration["AwsS3:SecretKey"]);

                var config = new AmazonS3Config
                { ServiceURL = _configuration["AwsS3:UrlAdress"] };

                _s3Client = new AmazonS3Client
                    (awsCredentials, config);

                var info = new S3FileInfo
                var getRequest = new GetPreSignedUrlRequest()
                {
                    BucketName = "projectmanagementsystem",
                    Key = request.FileKey,
                    Expires = DateTime.Now
                        .AddHours(request.LeaseTime)
                };

                var response = await _s3Client
                    .GetPreSignedURLAsync(getRequest);

                return new GetFileUrlServiceResponse(
                     Domain.Models.ServiceResponses.Base.ServiceResponseStatusBase.Success)
                {
                    Url = response
                };
            }
            catch (Exception ex)
            {
                _logger.LogError("GetFileUrlService : {Message}", ex.Message);

                return new GetFileUrlServiceResponse(
                     Domain.Models.ServiceResponses.Base.ServiceResponseStatusBase.InternalError);
            }
        }
    }
}
