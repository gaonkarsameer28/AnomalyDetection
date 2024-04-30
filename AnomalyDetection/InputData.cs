using Microsoft.ML.Data;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnomalyDetection
{
    public class InputData
    {
        [BsonElement("Id")]
        [LoadColumn(0)]
        public string Id;

        [BsonElement("Timestamp")]
        [LoadColumn(1)]
        public string Timestamp;

        [BsonElement("Value")]
        [LoadColumn(2)]
        public float Value;

        [BsonElement("Pvalue")]
        [LoadColumn(3)]
        public float Pvalue;

        [BsonElement("AnamolyDetected")]
        [LoadColumn(4)]
        public bool AnamolyDetected;
    }

    public class InputDataPrediction
    {
        public string Id;
        public string Timestamp;
        //vector to hold anomaly detection results. Including isAnomaly, anomalyScore, magnitude, expectedValue, boundaryUnits, upperBoundary and lowerBoundary.
        [VectorType(7)]
        public double[] Prediction { get; set; }
    }
}
