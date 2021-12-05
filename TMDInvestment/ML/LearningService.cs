using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using Microsoft.ML.Calibrators;
using Microsoft.ML.Runtime;
using Microsoft.ML.Transforms;
using TMDInvestment.Models;
using TMDInvestment.Services;
using Microsoft.ML.Transforms.TimeSeries;

namespace TMDInvestment.ML
{
    public class LearningService
    {
        string dataPath = string.Empty;
        //List<ModelInput> model = new List<ModelInput>();
        CoinBaseService service;
        TDAmeritradeService service2;
        dynamic error;
        public LearningService()
        {
            service = new CoinBaseService();
            service2 = new TDAmeritradeService();
        }

        public void Learn()
        {
            //dynamic historicData = service.GetHistoricData("BTC-USD", DateTime.Parse("08-01-2021"), DateTime.Parse("8-02-2021"), error);
            dynamic historicData = service2.GetPriceHistory("amc", PeriodType.month, 1, FrequencyType.daily, 1, ref error);
            //List<CandleItems> items = historicData.GetType().GetProperty("CandleItems");
            //var items = historicData.GetType();
            //List<CandleItems> items = historicData.GetType().GetProperty("CandleItems");
            Candles item = (Candles)historicData;
            //Step 1. Create an ML Context
            var ctx = new MLContext();

            //Step 2. Read in the input data from a text file for model training
            //IDataView trainingData = ctx.Data.LoadFromTextFile<ModelInput>(dataPath, hasHeader: true);
            //IDataView trainingData = ctx.Data.LoadFromEnumerable<ModelInput>(model);
            //IDataView trainingData = ctx.Data.LoadFromEnumerable<Candles>(historicData.ToArray());
            IDataView trainingData = ctx.Data.LoadFromEnumerable<CandleItems>(item.candles);

            // get an array of data points
            var sales = ctx.Data.CreateEnumerable<CandleItems>(trainingData, reuseRowObject: false).ToArray();

            //Step 3. Build your data processing and training pipeline
            //var pipeline = ctx.Transforms.Text
            //    .FeaturizeText("Features", nameof(SentimentIssue.Text))
            //    .Append(ctx.BinaryClassification.Trainers
            //        .LbfgsLogisticRegression("Label", "Features"));

            //var pipeline = ctx.Transforms.Concatenate("trend", "open").Append(ctx.Transforms.NormalizeMinMax("trend"));
            //var pipeline = ctx.Transforms.Concatenate("trend", "open").Append(ctx.Transforms.NormalizeMeanVariance("trend"));


            // build a training pipeline for detecting spikes
            var pipeline = ctx.Transforms.DetectIidSpike(
                outputColumnName: nameof(CandleItems.open),
                inputColumnName: nameof(ModelOutput.trend),
                confidence: 95,
                pvalueHistoryLength: sales.Count() / 4); // 25% of x-range

            #region Anomaly Detection and Prediction using ML.Net TimeSeries library
            ////Next, create an estimator to detect anomalies, we will use Singular Spectrum Analysis inside Transforms Catalog of ML Context and transform data desired to shape:
            //var pipelineAnomaly = ctx.Transforms.DetectSpikeBySsa(nameof(SpikeAnomaly.Anomalies), nameof(CurrencyModel.Close), confidence: 98.0, trainingWindowSize: 90, seasonalityWindowSize: 30, pvalueHistoryLength: 20);
            //var transformedData = pipelineAnomaly.Fit(data).Transform(data);

            ////Finally create a list to store detected anomalies:
            //var anomalies = ctx.Data.CreateEnumerable<SpikeAnomaly>(transformedData, reuseRowObject: false).ToList();

            ////Then write to console detected anomalies of given data:
            //var prices = data.GetColumn<float>("Close").ToArray();
            //var dates = data.GetColumn<DateTime>("Date").ToArray();
            //for (int i = 0; i < anomalies.Count; i++)
            //{
            //    if (anomalies[i].Anomalies[0] == 1)
            //    {
            //        Console.WriteLine($"{dates[i]}\t{prices[i]}");
            //    }
            //}

            ////We already loaded data to ML context so we only need to create a new estimator via forecasting singular spectrum analysis(SSA) function and create a model;
            //var pipelinePrediction = ctx.Forecasting.ForecastBySsa(nameof(PricePrediction.Predictions), nameof(CurrencyModel.Close), windowSize: 5, seriesLength: 10, trainSize: 100, horizon: 2);
            //var model = pipelinePrediction.Fit(data);

            ////Then create a prediction engine for the time series pipeline and call prediction function. You can also define the number of predictions that will be calculated. In our case, it will be 3, so the next 3 days of bitcoin prices will be predicted by using the prediction engine.
            //var forecastContext = model.CreateTimeSeriesEngine<CurrencyModel, PricePrediction>(context);
            //var forecasts = forecastContext.Predict(3); // Predict prices in next 3 days
            //foreach (var item in forecasts.Predictions)
            //{
            //    Console.WriteLine(item);
            //}
            #endregion
            //var forecastingPipeline = ctx.Forecasting.ForecastBySsa(
            //    outputColumnName: "ForecastedRentals",
            //    inputColumnName: "TotalRentals",
            //    windowSize: 7,
            //    seriesLength: 30,
            //    trainSize: 365,
            //    horizon: 7,
            //    confidenceLevel: 0.95f,
            //    confidenceLowerBoundColumn: "LowerBoundRentals",
            //    confidenceUpperBoundColumn: "UpperBoundRentals");

            //Step 4. Train your model
            ITransformer trainedModel = pipeline.Fit(trainingData);

            //Step 5. Make predictions using your trained model
            var predictionEngine = ctx.Model
                .CreatePredictionEngine<CandleItems, ModelOutput>(trainedModel);

            //var sampleStatement = new ModelInput() { price = 12.00 };
            var sampleStatement = new CandleItems() { open = 30.00m };

            var prediction = predictionEngine.Predict(sampleStatement);
            DataViewSchema output = predictionEngine.OutputSchema;
            
        }
    }
    //public class SentimentIssue
    //{
    //    public string Text { get; set; }
    //}

    //public class ModelInput {
    //    public string trend {get; set;}
    //    public double price { get; set; }
    //}

    public class ModelOutput {
        [ColumnName("trend"), LoadColumn(0)]
        public Single[] trend { get; set; }
    }

}
