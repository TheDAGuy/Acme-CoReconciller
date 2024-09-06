using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Util.Store;
using Google.Apis.Services;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Acme_CoReconciller.Google_Drive_Files
{
    public class Activity
    {
        private readonly DriveService _driveservice;
        private readonly UserCredential _credential;

        public Activity()
        {
            try
            {
                using (var stream = new FileStream("C:/Users/thead/Downloads/credentials.json", FileMode.Open, FileAccess.Read))
                {
                    _credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        new[] { DriveService.Scope.Drive },
                        "user",
                        CancellationToken.None,
                        new FileDataStore("Drive.Auth.Store")).Result;
                }

                _driveservice = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = _credential,
                    ApplicationName = "Acme&CoReconciller",
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing Google Drive service: {ex.Message}");
                throw;
            }
        }
        public async Task<List<Google.Apis.Drive.v3.Data.File>> ListFilesAsync(int pageSize = 10)
        {
            try 
            {
                var listRequest = _driveservice.Files.List();  
                listRequest.PageSize = pageSize;
                listRequest.Fields = "nextPageToken, files(id, name)";

                var result = await listRequest.ExecuteAsync();

                return result.Files.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting files from Google Drive: {ex.Message}");
                throw;
            }
        }

        public async Task<string> DownloadFileAsync(string fileId, string localFileName)
        {
            try
            {
                var file = _driveservice.Files.Get(fileId);

                using (var stream = new MemoryStream())
                {
                    await file.DownloadAsync(stream);
                    SaveStreamToFile(stream, localFileName);
                }

                return localFileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading file from Google Drive: {ex.Message}");
                throw;
            }
        }

        private void SaveStreamToFile(MemoryStream stream, string FilePath)
        {
            try
            {
                stream.Position = 0;
                using (var filestream = new FileStream(FilePath, FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(filestream);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file to local system: {ex.Message}");
                throw;
            }
        }

        public async Task UploadFile(string FilePath, string MimeType)
        {
            try
            {
                var FileMetaData = new Google.Apis.Drive.v3.Data.File()
                {
                    Name = Path.GetFileName(FilePath)
                };

                using (var stream = new FileStream(FilePath, FileMode.Open))
                {
                    var request = _driveservice.Files.Create(FileMetaData, stream, MimeType);
                    request.Fields = "id";
                    var file = await request.UploadAsync();

                    if (file.Status == Google.Apis.Upload.UploadStatus.Completed)
                    {
                        Console.WriteLine("Uploaded Successfully");
                    }
                    else
                    {
                        Console.WriteLine("Upload Failed!");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file to Google Drive: {ex.Message}");
                throw;
            }
        }
    }
}
