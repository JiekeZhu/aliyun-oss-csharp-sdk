﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using Aliyun.OSS.Common;
using Aliyun.OSS.Common.Authentication;
using Aliyun.OSS.Common.Communication;
using Aliyun.OSS.Common.Internal;
using Aliyun.OSS.Common.ThirdParty;
using Aliyun.OSS.Transform;
using Aliyun.OSS.Util;
using NUnit.Framework;

namespace Aliyun.OSS.Test.TestClass.OtherTestClass
{
    [TestFixture]
    public class ClassUnitTest
    {

        [OneTimeSetUp]
        public static void ClassInitialize()
        {
        }

        [OneTimeTearDown]
        public static void ClassCleanup()
        {
        }

        [Test]
        public void ServiceSignatureTest()
        {
            try
            {
                var signatuer = ServiceSignature.Create();
                signatuer.ComputeSignature("", "data");
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                var signatuer = ServiceSignature.Create();
                signatuer.ComputeSignature("key", "");
                Assert.IsTrue(false);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void MD5CoreTest()
        {
            try
            {
                MD5Core.GetHash(null, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHash("", null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                byte[] content = null;
                MD5Core.GetHash(content);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                byte[] content = null;
                MD5Core.GetHashString(content);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHashString(null, null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            try
            {
                MD5Core.GetHashString("", null);
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            string str32 = "";
            string str80 = "";
            string str124 = "";

            for (int i = 0; i < 32; i++)
                str32 += "a";
            for (int i = 0; i < 80; i++)
                str80 += "a";
            for (int i = 0; i < 124; i++)
                str124 += "a";

            MD5Core.GetHash(str32);
            MD5Core.GetHash(str80);
            MD5Core.GetHash(str124);
            MD5Core.GetHashString(str32);
            MD5Core.GetHashString(str80);
            MD5Core.GetHashString(str124);
        }


        [Test]
        public void WrapperStreamTest()
        {
            try
            {
                var warp0 = new WrapperStream(null);
            }
            catch (ArgumentNullException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }

            var content = new MemoryStream(Encoding.ASCII.GetBytes("123"));
            var warp1 = new WrapperStream(content);
            var warp2 = new WrapperStream(warp1);
            Assert.AreEqual(warp1.GetNonWrapperBaseStream(), content);
            Assert.AreEqual(warp2.GetNonWrapperBaseStream(), content);

            var seekWarp2 = warp2.GetSeekableBaseStream();

            Assert.AreEqual(WrapperStream.GetNonWrapperBaseStream(warp1), content);
            Assert.AreEqual(WrapperStream.GetNonWrapperBaseStream(content), content);

            Assert.AreEqual(warp1.CanWrite, true);

            warp1.Position = 2;
            Assert.AreEqual(content.Position, 2);

            try
            {
                warp1.ReadTimeout = 3;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var timeout = warp1.ReadTimeout;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                warp1.WriteTimeout = 3;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                var timeout = warp1.WriteTimeout;
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }


            warp1.Seek(0, SeekOrigin.Begin);
            Assert.AreEqual(content.Position, 0);

            warp1.SetLength(3);
            Assert.AreEqual(content.Length, 3);

        }

        [Test]
        public void PartialWrapperStreamTest()
        {
            var content = new MemoryStream(Encoding.ASCII.GetBytes("123"));
            content.Seek(1, SeekOrigin.Begin);
            var warp = new PartialWrapperStream(content, 0);
            Assert.AreEqual(warp.Length, 2);

            var warp1 = new PartialWrapperStream(content, 10);
            Assert.AreEqual(warp1.Length, 2);

            warp1.Position = 3;
            var ret = warp1.Read(null, 0, 0);
            Assert.AreEqual(ret, 0);

            warp1.Seek(0, SeekOrigin.Begin);
            warp1.Seek(1, SeekOrigin.Begin);
            warp1.Seek(4, SeekOrigin.Begin);
            warp1.Seek(0, SeekOrigin.Current);
            warp1.Seek(0, SeekOrigin.End);

            content.Seek(0, SeekOrigin.Begin);
            var md5Stream = new MD5Stream(content, null , content.Length);
            try
            {
                var warp2 = new PartialWrapperStream(md5Stream, 0);
                Assert.IsTrue(false);
            }
            catch (InvalidOperationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void HashingWrapperCrc64Test()
        {
            var hash = new HashingWrapperCrc64();
        }

        [Test]
        public void ServiceRequestTest()
        {
            string str;
            var request = new ServiceRequest();

            //BuildRequestUri
            request.Endpoint = new Uri("http://endpoint");
            request.ResourcePath = null;
            request.ParametersInUri = false;
            str = request.BuildRequestUri();
            Assert.AreEqual(str, "http://endpoint/");

            //BuildRequestContent
            var parameters = new Dictionary<string, string>();
            request.ParametersInUri = false;
            request.Content = null;
            request.Method = HttpMethod.Post;

            var stream = request.BuildRequestContent();
            Assert.AreEqual(stream, null);

            request.Dispose();
            request.Dispose();
        }

        [Test]
        public void DownloadObjectRequestTest()
        {
            var request = new DownloadObjectRequest("bucket", "key", "donwloadFile", "checkpointDir");
            request.NonmatchingETagConstraints.Add("etag1");
            request.NonmatchingETagConstraints.Add("etag2");

            //ToGetObjectRequest
            var objRequest = request.ToGetObjectRequest();
            Assert.AreEqual(objRequest.NonmatchingETagConstraints.Count, 2);


            //Populate
            var headers = new Dictionary<string, string>();
            request = new DownloadObjectRequest("bucket", "key", "donwloadFile", "checkpointDir");
            request.Populate(headers);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfModifiedSince), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfUnmodifiedSince), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfMatch), false);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfNoneMatch), false);

            request.ModifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);
            request.UnmodifiedSinceConstraint = DateTime.UtcNow.AddDays(-1);
            request.MatchingETagConstraints.Add("etag1");
            request.NonmatchingETagConstraints.Add("etag2");
            request.Populate(headers);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfModifiedSince), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfUnmodifiedSince), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfMatch), true);
            Assert.AreEqual(headers.ContainsKey(OssHeaders.GetObjectIfNoneMatch), true);

        }

        [Test]
        public void GeneratePresignedUriRequestTest()
        {
            var request = new GeneratePresignedUriRequest("bucket", "key");

            //Method
            request.Method = SignHttpMethod.Get;
            Assert.AreEqual(request.Method, SignHttpMethod.Get);
            request.Method = SignHttpMethod.Put;
            Assert.AreEqual(request.Method, SignHttpMethod.Put);
            try
            {
                request.Method = SignHttpMethod.Post;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //ResponseHeaders
            var responseHeader  = new ResponseHeaderOverrides();
            request.ResponseHeaders = responseHeader;
            Assert.AreEqual(request.ResponseHeaders, responseHeader);
            try
            {
                responseHeader = null;
                request.ResponseHeaders = responseHeader;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //QueryParams
            var queryParams = new Dictionary<string, string>();
            request.QueryParams = queryParams;
            Assert.AreEqual(request.QueryParams, queryParams);
            try
            {
                queryParams = null;
                request.QueryParams = queryParams;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void GetObjectRequestTest()
        {
            //Populate
            var headers = new Dictionary<string, string>();
            var request = new GetObjectRequest("bucket", "key");
            request.SetRange(-1, 0);
            request.Populate(headers);
            Assert.AreEqual(headers[HttpHeaders.Range], "bytes=-0");

            headers = new Dictionary<string, string>();
            request.SetRange(10, -1);
            request.Populate(headers);
            Assert.AreEqual(headers[HttpHeaders.Range], "bytes=10-");
        }

        [Test]
        public void SetBucketLifecycleRequestTest()
        {
            var request = new SetBucketLifecycleRequest("bucket");
            var rule = new LifecycleRule();
            rule.ID = "StandardExpireRule";
            rule.Prefix = "test";
            rule.Status = RuleStatus.Enabled;
            rule.ExpriationDays = 200;

            var rules = new List<LifecycleRule>();

            //LifecycleRules
            request.LifecycleRules = rules;
            try
            {
                request.LifecycleRules = null;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                for (int i = 0; i < 1002; i++)
                    rules.Add(rule);
                request.LifecycleRules = rules;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            //AddLifecycleRule
            try
            {
                request.AddLifecycleRule(null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                request = new SetBucketLifecycleRequest("bucket");
                var rule1 = new LifecycleRule();
                request.AddLifecycleRule(rule1);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }


            try
            {
                request = new SetBucketLifecycleRequest("bucket");
                for (int i = 0; i < 10002; i++)
                {
                    request.AddLifecycleRule(rule);
                }
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ObjectMetadataTest()
        {
            var meta = new ObjectMetadata();
            Assert.AreEqual(meta.LastModified, DateTime.MinValue);
            Assert.AreEqual(meta.ExpirationTime, DateTime.MinValue);
            meta.ExpirationTime = DateTime.UtcNow;
            Assert.AreNotEqual(meta.ExpirationTime, DateTime.MinValue);
            Assert.AreEqual(meta.ContentLength, -1L);
            meta.HttpMetadata.Remove(HttpHeaders.ContentLength);
            Assert.AreEqual(meta.ContentLength, 0);

            meta.ContentType = "";
            Assert.AreEqual(meta.ContentType, null);

            Assert.AreEqual(meta.ContentEncoding, null);
            meta.ContentEncoding = "gzip";
            Assert.AreEqual(meta.ContentEncoding, "gzip");

            meta.CacheControl = null;
            Assert.AreEqual(meta.CacheControl, null);
            meta.CacheControl = "data";
            Assert.AreEqual(meta.CacheControl, "data");

            meta.ContentDisposition = null;
            Assert.AreEqual(meta.ContentDisposition, null);
            meta.ContentDisposition = "data";
            Assert.AreEqual(meta.ContentDisposition, "data");

            meta.ETag = null;
            Assert.AreEqual(meta.ETag, null);
            meta.ETag = "data";
            Assert.AreEqual(meta.ETag, "data");

            meta.ContentMd5 = null;
            Assert.AreEqual(meta.ContentMd5, null);
            meta.ContentMd5 = "data";
            Assert.AreEqual(meta.ContentMd5, "data");

            meta.Crc64 = null;
            Assert.AreEqual(meta.Crc64, null);
            meta.Crc64 = "data";
            Assert.AreEqual(meta.Crc64, "data");

            //meta.ServerSideEncryption = null;
            Assert.AreEqual(meta.ServerSideEncryption, null);
            meta.ServerSideEncryption = "AES256";
            Assert.AreEqual(meta.ServerSideEncryption, "AES256");
            try
            {
                meta.ServerSideEncryption = "Unknwon";
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            Assert.AreEqual(meta.ObjectType, null);
            meta.HttpMetadata[HttpHeaders.ObjectType] = "Normal";
            Assert.AreEqual(meta.ObjectType, "Normal");

            //no httpmeta & usermeta
            meta = new ObjectMetadata();
            meta.HttpMetadata.Clear();
            HttpWebRequest req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //content-length ContentType null 
            meta = new ObjectMetadata();
            meta.HttpMetadata.Remove(HttpHeaders.ContentLength);
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //ContentLength  = 0 ContentType null 
            meta = new ObjectMetadata();
            meta.ContentLength = 0;
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);

            //ContentLength  > 0 ContentType not null 
            meta = new ObjectMetadata();
            meta.ContentLength = 10;
            meta.ContentType = "text/xml";
            meta.ContentMd5 = "data";
            meta.UserMetadata["user"] = "user";
            req = WebRequest.Create("http://www.endpoint/") as HttpWebRequest;
            meta.Populate(req);
        }

        [Test]
        public void OssObjectTest()
        {
            var obj = new OssObject();
            obj.Key = "key";

            obj.BucketName = null;
            var str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("targetBucket="), -1);

            obj.BucketName = "bucket";
            str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("targetBucket=bucket"), -1);
        }

        [Test]
        public void OwnerTest()
        {
            var obj = new Owner();

            var str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("Owner Id=,"), -1);
            Assert.AreNotEqual(str.IndexOf("DisplayName=]"), -1);

            obj.Id = "Id";
            obj.DisplayName = "DisplayName";
            str = obj.ToString();
            Assert.AreNotEqual(str.IndexOf("Owner Id=Id"), -1);
            Assert.AreNotEqual(str.IndexOf("DisplayName=DisplayName"), -1);
        }

        [Test]
        public void PolicyConditionsTest()
        {
            var policy = new PolicyConditions();

            var str = policy.Jsonize();
            Assert.AreNotEqual(str.IndexOf("conditions"), -1);

            //AddConditionItem
            policy.AddConditionItem(MatchMode.Exact, PolicyConditions.CondCacheControl, "data");
            policy.AddConditionItem(MatchMode.StartWith, PolicyConditions.CondContentEncoding, "data");

            policy.AddConditionItem("name", 10, 100);
            policy.AddConditionItem("eq-name", "value");
            Assert.IsTrue(true);

            str = policy.Jsonize();
            Assert.AreNotEqual(str.IndexOf("conditions"), -1);

            try
            {
                policy.AddConditionItem("name", 100, 10);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }


            try
            {
                policy.AddConditionItem(MatchMode.Range, PolicyConditions.CondContentLengthRange, "data");
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ResponseHeaderOverridesTest()
        {
            var respons = new ResponseHeaderOverrides();
            var parameters = new Dictionary<string, string>();

            respons.Populate(parameters);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseCacheControl), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseContentDisposition), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseContentEncoding), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderContentLanguage), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderContentType), false);
            Assert.AreEqual(parameters.ContainsKey(ResponseHeaderOverrides.ResponseHeaderExpires), false);

            respons.CacheControl = "Control";
            respons.ContentDisposition = "Disposition";
            respons.ContentEncoding = "Encoding";
            respons.ContentLanguage = "Language";
            respons.ContentType = "Type";
            respons.Expires = "Expires";
            respons.Populate(parameters);
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseCacheControl], "Control");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseContentDisposition], "Disposition");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseContentEncoding], "Encoding");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderContentLanguage], "Language");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderContentType], "Type");
            Assert.AreEqual(parameters[ResponseHeaderOverrides.ResponseHeaderExpires], "Expires");
        }

        [Test]
        public void ResumableContextTest()
        {
            var ctx = new ResumableContext("bucket", "key", "checkpointDir");
            ctx.Clear();
            ctx.Load();
            ctx.Dump();

            ctx = new ResumableContext("bucket", "key", "");
            ctx.Clear();
            ctx.Load();
            ctx.Dump();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString("id:md5:crc:"), false);

            Assert.AreEqual(ctx.ToString(), "");
            ctx.UploadId = "upload-id";
            ctx.PartContextList = new List<ResumablePartContext>();
            Assert.AreEqual(ctx.ToString(), "");

            ctx.PartContextList.Add(new ResumablePartContext());
            Assert.AreEqual(ctx.ToString(), "upload-id:md5:crc:0_0_0_False___0");


            try
            {
                string checkdir = "";
                for (int i = 0; i < 256; i++)
                    checkdir += "a";

                ctx = new ResumableContext("bucket", "key", checkdir); 
                var path = ctx.CheckpointFile;
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void ResumableDownloadContextTest()
        {
            var ctx = new ResumableDownloadContext("bucket", "key", "checkpointDir");
            ctx.Clear();
            ctx.Load();

            ctx = new ResumableDownloadContext("bucket", "key", "");
            ctx.Clear();
            ctx.Load();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString(":::"), false);
            Assert.AreEqual(ctx.FromString("etag:MD5:1234:1,2,3"), false);

            ctx.PartContextList = new List<ResumablePartContext>();
            Assert.AreEqual(ctx.FromString("etag:MD5:1234:1_2_3_True___0,2_2_3_True___0"), true);
        }

        [Test]
        public void ResumablePartContextTest()
        {
            var ctx = new ResumablePartContext();

            Assert.AreEqual(ctx.FromString(""), false);
            Assert.AreEqual(ctx.FromString("1_2"), false);
            Assert.AreEqual(ctx.FromString("-1_2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("a_2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_-2_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_a_3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_-3_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_a_4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_-4_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_a_5_6_7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4___7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_-1__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_a__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_5__7"), false);
            Assert.AreEqual(ctx.FromString("1_2_3_4_5_7_a"), false);


            ctx = new ResumablePartContext();
            ctx.PartId = 1;
            ctx.Position = 2;
            ctx.Length = 3;
            ctx.IsCompleted = true;
            ctx.Crc64 = 0;
            Assert.AreEqual(ctx.ToString(), "1_2_3_True___0");
        }

        [Test]
        public void LifecycleRuleTest()
        {
            var rule1 = new LifecycleRule();
            var rule2 = new LifecycleRule();
            var rule1f = rule1;

            Assert.AreEqual(rule1.Equals(rule1f), true);
            Assert.AreEqual(rule1.Equals(null), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test";
            Assert.AreEqual(rule1.Equals(rule2), true);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID1";
            rule2.Prefix = "test";
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test1";
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.ID = "RuleID";
            rule1.Prefix = "test";
            rule2.ID = "RuleID";
            rule2.Prefix = "test";
            rule1.ExpriationDays = 200;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.ExpriationDays = rule1.ExpriationDays;
            rule1.ExpirationTime = DateTime.UtcNow;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.ExpirationTime = rule1.ExpirationTime;
            rule1.CreatedBeforeDate = DateTime.UtcNow;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.CreatedBeforeDate = rule1.CreatedBeforeDate;
            rule1.Status = RuleStatus.Enabled;
            rule2.Status = RuleStatus.Disabled;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.Status = rule1.Status;
            rule1.AbortMultipartUpload = null;
            rule2.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration();
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.Status = rule1.Status;
            rule1.AbortMultipartUpload = new LifecycleRule.LifeCycleExpiration();
            rule1.AbortMultipartUpload.Days = 20;
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule2.AbortMultipartUpload = rule1.AbortMultipartUpload;
            rule1.Transitions = null;
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[2];
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2];
            rule2.Transitions = null; 
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2];
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[1];
            Assert.AreEqual(rule1.Equals(rule2), false);

            rule1.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.IA
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            rule2.Transitions = new LifecycleRule.LifeCycleTransition[2]
            {
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                },
                new LifecycleRule.LifeCycleTransition(){
                    StorageClass = StorageClass.Archive
                }
            };
            Assert.AreEqual(rule1.Equals(rule2), false);


            //LifeCycleExpiration
            var expiration1 = new LifecycleRule.LifeCycleExpiration();
            var expiration2 = new LifecycleRule.LifeCycleExpiration();

            Assert.AreEqual(expiration1.Equals(expiration1), true);
            Assert.AreEqual(expiration1.Equals(expiration2), true);
            Assert.AreEqual(expiration1.Equals(null), false);

            expiration1.Days = 5;
            expiration2.Days = 10;
            Assert.AreEqual(expiration1.Equals(expiration2), false);

            expiration1.Days = 10;
            expiration2.Days = 10;
            expiration1.CreatedBeforeDate = DateTime.UtcNow;
            Assert.AreEqual(expiration1.Equals(expiration2), false);

            //LifeCycleTransition
            var transition1 = new LifecycleRule.LifeCycleTransition();
            var transition2 = new LifecycleRule.LifeCycleTransition();

            Assert.AreEqual(transition1.Equals(null), false);
            Assert.AreEqual(transition1.Equals(transition2), true);

            transition1.StorageClass = StorageClass.Archive;
            transition2.StorageClass = StorageClass.Standard;
            Assert.AreEqual(transition1.Equals(transition2), false);
        }

        [Test]
        public void OssClientTest()
        {
            try
            {
                Uri uri = null;
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(uri, credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(new Uri("rtmp://endpoint"), credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            try
            {
                ICredentialsProvider credsProvider = null;
                var client = new OssClient(new Uri("http://endpoint"), credsProvider, null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            var client1 = new OssClient("endpoint", "ak", "sk");
            var credentials = new DefaultCredentials("ak", "sk", null);
            client1.SwitchCredentials(credentials);
            client1.SetEndpoint(new Uri("https://endpoint"));

            try
            {
                client1.SwitchCredentials(null);
                Assert.IsTrue(false);
            }
            catch
            {
                Assert.IsTrue(true);
            }

            Common.ClientConfiguration conf = null;
            client1 = new OssClient(new Uri("http://endpoint"), "ak", "sk", conf);
            client1.SwitchCredentials(credentials);
        }

        [Test]
        public void ListObjectsRequestTest()
        {
            var request = new ListObjectsRequest("bucket");

            //perfix 
            try
            {
                request.Prefix = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Prefix = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //marker
            try
            {
                request.Marker = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Marker = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //MaxKeys
            try
            {
                request.MaxKeys = 1024;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            //Delimiter
            try
            {
                request.Delimiter = null;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                string str = "a";
                for (int i = 0; i < 1024; i++)
                    str += "b";
                request.Delimiter = str;
            }
            catch (ArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }

            request.EncodingType = null;
            Assert.AreEqual(request.EncodingType, HttpUtils.UrlEncodingType);
            request.EncodingType = "type";
            Assert.AreEqual(request.EncodingType, "type");
        }

        [Test]
        public void AccessControlListTest()
        {
            var acl = new AccessControlList();

            try
            {
                acl.GrantPermission(null, Permission.FullControl);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            try
            {
                acl.RevokeAllPermissions(null);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }

            var grantee1 = GroupGrantee.AllUsers;
            var grantee2 = GroupGrantee.AllUsers;
            Assert.AreEqual(grantee1, grantee2);
            acl.GrantPermission(grantee1, Permission.FullControl);

            try
            {
                acl.RevokeAllPermissions(grantee2);
            }
            catch
            {
            }
        }

        [Test]
        public void DeserializerFactoryTest()
        {
            var factory = DeserializerFactory.GetFactory("text/json");
            Assert.AreEqual(factory, null);

            factory = DeserializerFactory.GetFactory("text/xml");
            Assert.AreNotEqual(factory.CreateUploadPartResultDeserializer(1), null);
        }

        internal class ResponseMock : ServiceResponse
        {
            public IDictionary<string, string> _headers;
            public HttpStatusCode _statusCode;
            public Stream _stream;

            public override HttpStatusCode StatusCode
            {
                get { return _statusCode; }
            }

            public override Exception Failure
            {
                get { return null; }
            }

            public override IDictionary<string, string> Headers
            {
                get
                {
                    return _headers;
                }
            }

            public override Stream Content
            {
                get
                {
                    return _stream;
                }
            }

            public ResponseMock()
            {
            }

            public ResponseMock(HttpStatusCode code, IDictionary<string, string> Headers, Stream stream)
            {
                _statusCode = code;
                _headers = Headers;
                _stream = stream;
            }

            private static IDictionary<string, string> GetResponseHeaders(HttpWebResponse response)
            {
                var headers = response.Headers;
                var result = new Dictionary<string, string>(headers.Count);

                for (var i = 0; i < headers.Count; i++)
                {
                    var key = headers.Keys[i];
                    var value = headers.Get(key);
                    result.Add(key, HttpUtils.Reencode(value, HttpUtils.Iso88591Charset, HttpUtils.Utf8Charset));
                }

                return result;
            }
        }

        internal class ServiceClientMock : ServiceClient
        {
            public ServiceClientMock(ClientConfiguration configuration)
            : base(configuration)
            {
            }

            protected override IAsyncResult BeginSendCore(ServiceRequest request, ExecutionContext context, AsyncCallback callback, object state)
            {
                throw new NotImplementedException();
            }

            protected override ServiceResponse SendCore(ServiceRequest request, ExecutionContext context)
            {
                throw new NotImplementedException();
            }
        }
            

        [Test]
        public void AppendObjectResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateAppendObjectReusltDeserializer();
            var headers = new Dictionary<string, string>();
            var content = new MemoryStream();
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetAclResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetAclResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data = 
                @" 
                <AccessControlPolicy>
                    <Owner>
                        <ID>0022012****</ID>
                        <DisplayName>user_example</DisplayName>
                    </Owner>
                    <AccessControlList>
                        <Grant>default</Grant>
                    </AccessControlList>
                </AccessControlPolicy>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.ACL, CannedAccessControlList.Default);

            data =
                @" 
                <AccessControlPolicy>
                    <Owner>
                        <ID>0022012****</ID>
                        <DisplayName>user_example</DisplayName>
                    </Owner>
                    <AccessControlList>
                        <Grant>unknown</Grant>
                    </AccessControlList>
                </AccessControlPolicy>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.ACL, CannedAccessControlList.Private);
        }

        [Test]
        public void GetBucketLifecycleDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetBucketLifecycleDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Enabled</Status>
                    <Expiration>
                      <Date>2017-01-01T00:00:00.000Z</Date>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Count, 1);

            data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Enabled</Status>
                    <Expiration>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <LifecycleConfiguration>
                  <Rule>
                    <ID>delete after one day</ID>
                    <Prefix>logs/</Prefix>
                    <Status>Unknown</Status>
                    <Expiration>
                      <Days>1</Days>
                    </Expiration>
                  </Rule>
                </LifecycleConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (InvalidEnumArgumentException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void GetCorsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateGetCorsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <CORSConfiguration>
                    <CORSRule>
                      <AllowedOrigin></AllowedOrigin>
                      <AllowedMethod></AllowedMethod>
                      <AllowedHeader>*</AllowedHeader>
                      <ExposeHeader>x-oss-test</ExposeHeader>
                      <MaxAgeSeconds>100</MaxAgeSeconds>
                    </CORSRule>
                </CORSConfiguration>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <CORSConfiguration>
                    <CORSRule>
                      <AllowedHeader>*</AllowedHeader>
                      <ExposeHeader>x-oss-test</ExposeHeader>
                      <MaxAgeSeconds>100</MaxAgeSeconds>
                    </CORSRule>
                </CORSConfiguration>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetObjectResponseDeserializerTest()
        {
            var clientService = new ServiceClientMock(new ClientConfiguration());
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new GetObjectRequest("bucket", "key");
            var deserializer = factory.CreateGetObjectResultDeserializer(request, new RetryableServiceClient(clientService));
            var headers = new Dictionary<string, string>();
            headers.Add(HttpHeaders.HashCrc64Ecma, "abcdef");
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetSymlinkResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = new GetSymlinkResultDeserializer();// factory.sy();
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                var result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (Common.OssException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void ListMultipartUploadsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListMultipartUploadsResultDeserializer();// factory.sy();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListMultipartUploadsResult>
                    <Bucket>oss-example</Bucket>
                    <KeyMarker></KeyMarker>
                    <UploadIdMarker></UploadIdMarker>
                    <NextKeyMarker>oss.avi</NextKeyMarker>
                    <NextUploadIdMarker>0004B99B8E707874FC2D692FA5D77D3F</NextUploadIdMarker>
                    <Delimiter></Delimiter>
                    <Prefix></Prefix>
                    <MaxUploads>1000</MaxUploads>
                    <IsTruncated>false</IsTruncated>
                    <CommonPrefixes>
                        <Prefix>multipart.data</Prefix>
                    </CommonPrefixes>
                </ListMultipartUploadsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <ListMultipartUploadsResult>
                    <Bucket>oss-example</Bucket>
                    <KeyMarker></KeyMarker>
                    <UploadIdMarker></UploadIdMarker>
                    <NextKeyMarker>oss.avi</NextKeyMarker>
                    <NextUploadIdMarker>0004B99B8E707874FC2D692FA5D77D3F</NextUploadIdMarker>
                    <Delimiter></Delimiter>
                    <Prefix></Prefix>
                    <MaxUploads>1000</MaxUploads>
                    <IsTruncated>false</IsTruncated>
                    <CommonPrefixes>
                    </CommonPrefixes>
                </ListMultipartUploadsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void ListObjectsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListObjectsResultDeserializer();;
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListBucketResult>
                    <Name>oss-example</Name>
                    <Prefix></Prefix>
                    <Marker></Marker>
                    <MaxKeys>100</MaxKeys>
                    <Delimiter></Delimiter>
                    <IsTruncated>false</IsTruncated>
                    <Contents>
                          <Key>fun/movie/001.avi</Key>
                          <LastModified>2012-02-24T08:43:07.000Z</LastModified>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                          <Owner>
                              <ID>0022012****</ID>
                              <DisplayName>user-example</DisplayName>
                          </Owner>
                    </Contents>
                    <Contents>
                          <Key>fun/movie/007.avi</Key>
                          <LastModified>2012-02-24T08:43:27.000Z</LastModified>
                          <ETag>5B3C1A2E053D763E1B002CC607C5A0FE1****</ETag>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                          <Owner>
                          </Owner>
                    </Contents>
                    <Contents>
                          <Key>fun/movie/007.avi</Key>
                          <LastModified>2012-02-24T08:43:27.000Z</LastModified>
                          <ETag>5B3C1A2E053D763E1B002CC607C5A0FE1****</ETag>
                          <Type>Normal</Type>
                          <Size>344606</Size>
                          <StorageClass>Standard</StorageClass>
                    </Contents>
                    <CommonPrefixes>
                    </CommonPrefixes>
                </ListBucketResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void ListPartsResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateListPartsResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <ListPartsResult>
                    <Bucket>multipart_upload</Bucket>
                    <Key>multipart.data</Key>
                    <UploadId>0004B999EF5A239BB9138C6227D69F95</UploadId>
                    <NextPartNumberMarker></NextPartNumberMarker>
                    <MaxParts>1000</MaxParts>
                    <IsTruncated>false</IsTruncated>
                    <Part>
                        <PartNumber>1</PartNumber>
                        <LastModified>2012-02-23T07:01:34.000Z</LastModified>
                        <Size>6291456</Size>
                    </Part>
                </ListPartsResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);

            data =
                @" 
                <ListPartsResult>
                    <Bucket>multipart_upload</Bucket>
                    <Key>multipart.data</Key>
                    <UploadId>0004B999EF5A239BB9138C6227D69F95</UploadId>
                    <NextPartNumberMarker></NextPartNumberMarker>
                    <MaxParts>1000</MaxParts>
                    <IsTruncated>false</IsTruncated>
                    <EncodingType></EncodingType>
                </ListPartsResult>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void PutObjectResponseDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreatePutObjectReusltDeserializer(request);
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            //empty
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void UploadPartCopyResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateUploadPartCopyResultDeserializer(1);
            var headers = new Dictionary<string, string>();
            headers.Add(HttpHeaders.QuotaDeltaSize, "100");
            string data =
                @" 
                <CopyPartResult>
                    <LastModified>2014-07-17T06:27:54.000Z </LastModified>
                    <ETag>5B3C1A2E053D763E1B002CC607C5****</ETag>
                </CopyPartResult>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);


            //invalid xml
            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);

            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void UploadPartResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var deserializer = factory.CreateUploadPartResultDeserializer(1);
            var headers = new Dictionary<string, string>();
            string data = "";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
        }

        [Test]
        public void GetBucketTaggingResultDeserializerTest()
        {
            var factory = DeserializerFactory.GetFactory("text/xml");
            var request = new PutObjectRequest("bucket", "key", new MemoryStream());
            var deserializer = factory.CreateGetBucketTaggingResultDeserializer();
            var headers = new Dictionary<string, string>();
            string data =
                @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                            <Value>jsmith</Value>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            var content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            var xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            var result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, "jsmith");

            data =
             @" 
                <Tagging>
                    <TagSet>
                        <Tag>
                            <Key>tag2</Key>
                        </Tag>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 1);
            Assert.AreEqual(result.Tags[0].Key, "tag2");
            Assert.AreEqual(result.Tags[0].Value, null);

            data =
                @" 
                <Tagging>
                    <TagSet>
                    </TagSet>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data =
                @" 
                <Tagging>
                </Tagging>
                ";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            result = deserializer.Deserialize(xmlStream);
            Assert.AreEqual(result.Tags.Count, 0);

            data = "invalid xml";
            content = new MemoryStream(Encoding.ASCII.GetBytes(data));
            xmlStream = new ResponseMock(HttpStatusCode.OK, headers, content);
            try
            {
                result = deserializer.Deserialize(xmlStream);
                Assert.IsTrue(false);
            }
            catch (ResponseDeserializationException e)
            {
                Assert.IsTrue(true, e.Message);
            }
            catch (Exception e)
            {
                Assert.IsTrue(false, e.Message);
            }
        }

        [Test]
        public void SerializerFactoryTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            Assert.AreNotEqual(factory, null);

            factory = SerializerFactory.GetFactory(null);
            Assert.AreNotEqual(factory, null);

            factory = SerializerFactory.GetFactory("text/json");
            Assert.AreEqual(factory, null);
        }

        [Test]
        public void SetBucketCorsRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSetBucketCorsRequestSerializer();

            var rules = new List<CORSRule>();
            var rule = new CORSRule();
            //rule.AllowedHeaders = null;
            rule.AllowedMethods = null;
            //rule.AllowedOrigins = null;
            //rule.ExposeHeaders = null;
            rules.Add(new CORSRule());
            var request = new SetBucketCorsRequest("bucket");
            request.CORSRules = rules;
            var result = serializer.Serialize(request);
            Assert.AreNotEqual(result, null);
        }

        [Test]
        public void SetBucketLifecycleRequestSerializerTest()
        {
            var factory = SerializerFactory.GetFactory("text/xml");
            var serializer = factory.CreateSetBucketLifecycleRequestSerializer();

            var request = new SetBucketLifecycleRequest("bucket");
            var result = serializer.Serialize(request);
        }

        [Test]
        public void XmlStreamSerializerTest()
        {
            var streamSerializer = new XmlStreamSerializer<ResponseMock>();
            try { 
                streamSerializer.Serialize(null);
                Assert.IsTrue(false);
            }
            catch (Exception e)
            {
                Assert.IsTrue(true, e.Message);
            }
        }

        [Test]
        public void EndTest()
        {
            var hash = new HashingWrapperCrc64();
        }
    }
}