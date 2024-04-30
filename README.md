# Anomaly Detection

This repository provides code for anomaly detection algorithms, enabling you to identify unusual patterns or data points that deviate significantly from the expected behavior. Anomaly detection has a wide range of applications in various domains, including:

## Use Case: 
Transaction Monitoring: This code is implemented for transaction monitoring to find anomalies . Anomalies can be in number of incoming transactions  vs sucusseffull vs failed transactions . It keeps historical data to train the model & use input data find anomalies as per the latest trend .


## Prerequisites:

.Net core framework 
Visual studio
 
## How to use
Clone this repository using git clone https://github.com/gaonkarsameer28/AnomalyDetection.git.
Build the class libabriray in visual studio .
call the method DetectSpike (MLContext mlContext, int docSize, IDataView productSales, List<InputData> DataModel).
method can be called from console or api application 
