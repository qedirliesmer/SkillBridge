using Microsoft.Extensions.Options;
using Minio.DataModel.Args;
using Minio;
using SkillBridge.Application.Abstracts.Services;
using SkillBridge.Application.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkillBridge.Infrastructure.Services;

public class S3MinioFileStorageService : IFileStorageService
{
    private readonly IMinioClient _minioClient;
    private readonly MinioOptions _options;

    public S3MinioFileStorageService(IMinioClient minioClient, IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _options = options.Value;
    }

    public async Task<string> SaveAsync(Stream content, string fileName, string contentType, int propertyAdId, CancellationToken ct = default)
    {
        var bucketExistsArgs = new BucketExistsArgs().WithBucket(_options.Bucket);
        bool found = await _minioClient.BucketExistsAsync(bucketExistsArgs, ct);

        if (!found)
        {
            var makeBucketArgs = new MakeBucketArgs().WithBucket(_options.Bucket);
            await _minioClient.MakeBucketAsync(makeBucketArgs, ct);
        }

        var extension = Path.GetExtension(fileName);
        var objectKey = $"skills/{propertyAdId}/{Guid.NewGuid()}{extension}";

        var putObjectArgs = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType);

        await _minioClient.PutObjectAsync(putObjectArgs, ct);

        return objectKey;
    }

    public async Task DeleteFileAsync(string objectKey, CancellationToken ct = default)
    {
        var removeObjectArgs = new RemoveObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectKey);

        await _minioClient.RemoveObjectAsync(removeObjectArgs, ct);
    }
}