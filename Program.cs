using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using System.Text;

namespace YnuClassificationPrediction
{
    class PredictionConsole
    {
        // 아래코드는 수정 X
        static readonly string Endpoint = "https://ynucustomvision-prediction.cognitiveservices.azure.com/";
        static readonly string PredictionKey = "51e9a0bde7444cc3892cf923145593cb";
        static readonly string ProjectId = "639ae309-652b-4c79-9612-86d378576638";
        static readonly string PublishedName = "YnuClassificationModel";

        // Classification 전 이미지의 폴더 경로 --------------------------- 사용에 따라 수정
        static readonly string TestImageFolder = @"C:\Users\최원석\source\repos\YnuClassificationPrediction\Images\";

        // Classification 후 결과가 담길 폴더 경로(csv 파일명까지) --------------------------- 사용에 따라 수정
        static readonly string ResultFolder = @"C:\Users\최원석\source\repos\YnuClassificationPrediction\Result\classification_result.csv";

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