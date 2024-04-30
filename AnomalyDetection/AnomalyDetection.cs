using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection
{
    public class AnomalyDetection
    {
        // <SnippetDeclareGlobalVariables>
        
        //assign the Number of records in dataset file to constant variable
        int _docsize = 0;
        // </SnippetDeclareGlobalVariables>
        public List<InputData> DetectAnamoly(List<InputData> DataModel)
        {
            // Create MLContext to be shared across the model creation workflow objects
            // <SnippetCreateMLContext>           
            MLContext DataModelmlContext = new MLContext();
            // </SnippetCreateMLContext>

            //STEP 1: Common data loading configuration
            // <SnippetLoadData>
            //IDataView dataView = mlContext.Data.LoadFromTextFile<InputData>(path: _dataPath, hasHeader: true, separatorChar: ',');

            IDataView ModeldataView = DataModelmlContext.Data.LoadFromEnumerable(DataModel);
            // </SnippetLoadData>
            _docsize = DataModel.Count;
            // Spike detects pattern temporary changes
            // <SnippetCallDetectSpike>
            DetectSpike(DataModelmlContext, _docsize, ModeldataView, DataModel);
            // </SnippetCallDetectSpike>

            // Changepoint detects pattern persistent changes
            // <SnippetCallDetectChangepoint>
            //  DetectChangepoint(mlContext, _docsize, dataView);
            // </SnippetCallDetectChangepoint>
            return DataModel;
        }
        public List<InputData> DetectSpike(MLContext mlContext, int docSize, IDataView productSales, List<InputData> DataModel)
        {
            Console.WriteLine("Detect temporary changes in pattern");


            // STEP 2: Set the training algorithm
            // <SnippetAddSpikeTrainer>
            //var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(InputDataPrediction.Prediction), inputColumnName: nameof(InputData.numSales), confidence: 95, pvalueHistoryLength: docSize / 4);
            var iidSpikeEstimator = mlContext.Transforms.DetectIidSpike(outputColumnName: nameof(InputDataPrediction.Prediction), inputColumnName: nameof(InputData.Value), confidence: 95.00, pvalueHistoryLength: docSize / 4);
            // </SnippetAddSpikeTrainer>

            // STEP 3: Create the transform
            // Create the spike detection transform
            Console.WriteLine("=============== Training the model ===============");
            // <SnippetTrainModel1>
            ITransformer iidSpikeTransform = iidSpikeEstimator.Fit(CreateEmptyDataView(mlContext));
            // </SnippetTrainModel1>

            Console.WriteLine("=============== End of training process ===============");
            //Apply data transformation to create predictions.
            // <SnippetTransformData1>
            IDataView transformedData = iidSpikeTransform.Transform(productSales);
            // </SnippetTransformData1>

            // <SnippetCreateEnumerable1>
            var predictions = mlContext.Data.CreateEnumerable<InputDataPrediction>(transformedData, reuseRowObject: false);
            // </SnippetCreateEnumerable1>

            // <SnippetDisplayHeader1>
            Console.WriteLine("Alert\tScore\tP-Value");
            // </SnippetDisplayHeader1>

            // <SnippetDisplayResults1>
            foreach (var p in predictions)
            {
                // DataModel.Where(dm => dm.Id == p.Id).Select(c => { c.Pvalue = float.Parse(p.Prediction[2].ToString());return c; });
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- Spike detected";
                    DataModel.Where(dm => dm.Id == p.Id).Select(c => { c.AnamolyDetected = true; return c; }).ToList();
                }

                Console.WriteLine(results);
            }
            Console.WriteLine("");
            // </SnippetDisplayResults1>
            return DataModel;
        }

        private void DetectChangepoint(MLContext mlContext, int docSize, IDataView productSales)
        {
            Console.WriteLine("Detect Persistent changes in pattern");

            //STEP 2: Set the training algorithm
            // <SnippetAddChangePointTrainer>
            //var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(InputDataPrediction.Prediction), inputColumnName: nameof(InputData.numSales), confidence: 95, changeHistoryLength: docSize / 4);
            var iidChangePointEstimator = mlContext.Transforms.DetectIidChangePoint(outputColumnName: nameof(InputDataPrediction.Prediction), inputColumnName: nameof(InputData.Value), confidence: 95.00, changeHistoryLength: docSize / 4);
            // </SnippetAddChangePointTrainer>

            //STEP 3: Create the transform
            Console.WriteLine("=============== Training the model Using Change Point Detection Algorithm===============");
            // <SnippetTrainModel2>
            var iidChangePointTransform = iidChangePointEstimator.Fit(CreateEmptyDataView(mlContext));
            // </SnippetTrainModel2>
            Console.WriteLine("=============== End of training process ===============");

            //Apply data transformation to create predictions.
            // <SnippetTransformData2>
            IDataView transformedData = iidChangePointTransform.Transform(productSales);
            // </SnippetTransformData2>

            // <SnippetCreateEnumerable2>
            var predictions = mlContext.Data.CreateEnumerable<InputDataPrediction>(transformedData, reuseRowObject: false);
            // </SnippetCreateEnumerable2>

            // <SnippetDisplayHeader2>
            Console.WriteLine("Alert\tScore\tP-Value\tMartingale value");
            // </SnippetDisplayHeader2>

            // <SnippetDisplayResults2>
            foreach (var p in predictions)
            {
                var results = $"{p.Prediction[0]}\t{p.Prediction[1]:f2}\t{p.Prediction[2]:F2}\t{p.Prediction[3]:F2}";

                if (p.Prediction[0] == 1)
                {
                    results += " <-- alert is on, predicted changepoint";
                }
                Console.WriteLine(results);
            }
            Console.WriteLine("");
            // </SnippetDisplayResults2>
        }

        // <SnippetCreateEmptyDataView>
        static IDataView CreateEmptyDataView(MLContext mlContext)
        {
            // Create empty DataView. We just need the schema to call Fit() for the time series transforms
            IEnumerable<InputData> enumerableData = new List<InputData>();
            return mlContext.Data.LoadFromEnumerable(enumerableData);
        }
        // </SnippetCreateEmptyDataView>
    }
}
