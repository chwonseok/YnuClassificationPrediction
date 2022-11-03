using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Text;

namespace YnuClassificationPrediction
{
    class PredictionConsole
    {
        // 1번 여기를 먼저 셋업해 주세요
        static readonly string Endpoint = "Endpoint";
        static readonly string PredictionKey = "PredictionKey";
        static readonly string ProjectId = "ProjectId";
        static readonly string PublishedName = "PublishedName";

        // 2번 Classification 전 이미지의 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string TestImageFolder = @"사용자의 Images 폴더 경로";

        // 3번 Classification 후 결과가 담길 폴더 경로(csv 파일명까지) --------------------------- 사용에 따라 수정
        static readonly string ResultFolder = @"사용자의 Result 폴더 내 csv 파일명 및 경로";

        static async Task Main(string[] args)
        {
            // Get each image path
            var imagesPaths = Directory.GetFiles(TestImageFolder);

            var predictionCount = 0;

            var allCsvResult = new StringBuilder();

            foreach (var imagePath in imagesPaths)
            {
                var csvResult = new StringBuilder();

                // Get each file name
                var fileName = Path.GetFileName(imagePath);

                // Get predictions of each image
                var imagePrediction = await GetImagePredictionsAsync(imagePath);

                foreach (var prediction in imagePrediction.Predictions)
                {
                    var aLine = $"{fileName},{prediction.TagName},{prediction.Probability}";
                    csvResult.AppendLine(aLine);
                }

                allCsvResult.AppendLine(csvResult.ToString());
                Console.WriteLine($"filename: {fileName}, Prediction Completed");
                predictionCount++;
            }

            await File.WriteAllTextAsync(ResultFolder, allCsvResult.ToString());
            Console.WriteLine($"{predictionCount}개 이미지 대상 Prediction 완료");
        }

        // Authenticate the prediction
        private static CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            CustomVisionPredictionClient predictionApi = new(new ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }

        static async Task<ImagePrediction> GetImagePredictionsAsync(string imagePath)
        {
            // Get predictions from the image
            using (var imageStream = new FileStream(imagePath, FileMode.Open))
            {
                var predictionApi = AuthenticatePrediction(Endpoint, PredictionKey);

                var predictions = await predictionApi.ClassifyImageWithNoStoreAsync
                    (new Guid(ProjectId), PublishedName, imageStream);
                return predictions;
            };
        }
    }
}