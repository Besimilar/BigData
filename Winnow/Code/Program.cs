using System;
using System.IO;

namespace winnow
{
    public class Winnow
    {
        private int numInput;
        private double[] weights;
        private double threshold; // to determine Y = 0 or 1
        private double alpha; // increase/decrase factor
        private static Random rnd;

        public Winnow(int numInput, int rndSeed)
        {
            this.numInput = numInput;
            this.weights = new double[numInput];
            for (int i = 0; i < weights.Length; ++i)
                weights[i] = numInput / 2.0;
            this.threshold = 1.0 * numInput;
            this.alpha = 2.0;
            rnd = new Random(rndSeed);
        }

        public int ComputeY(int[] xValues)
        {
            double sum = 0.0;
            for (int i = 0; i < numInput; ++i)
                sum += weights[i] * xValues[i];
            if (sum > this.threshold)
                return 1;
            else
                return 0;
        }

        public double[] TrainWeights(int[][] trainData)
        {
            int[] xValues = new int[numInput];
            int target;
            int computed;

			// ********* include Y??? all random
            ShuffleObservations(trainData); // random shuffle


            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, numInput); // get the inputs
                target = trainData[i][numInput]; // last value is target
                computed = ComputeY(xValues);

                if (computed == 1 && target == 0) // need to decrease weights
                {
                    for (int j = 0; j < numInput; ++j)
                    {
                        if (xValues[j] == 0) continue; // no change when xi = 0
                        weights[j] = weights[j] / alpha; // demotion
                    }
                }
                else if (computed == 0 && target == 1) // need to increase weights
                {
                    for (int j = 0; j < numInput; ++j)
                    {
                        if (xValues[j] == 0) continue; // no change when xi = 0
                        weights[j] = weights[j] * alpha; // promotion
                    }
                }
            } // each training item

            double[] result = new double[numInput]; // = number weights
            Array.Copy(this.weights, result, numInput);
            return result;
        } // Train

        private static void ShuffleObservations(int[][] trainData) // Fisher-Yates shuffle algorithm
        {
            for (int i = 0; i < trainData.Length; ++i)
            {
                int r = rnd.Next(i, trainData.Length);
                int[] tmp = trainData[r];
                trainData[r] = trainData[i];
                trainData[i] = tmp;
            }
        }

        public double Accuracy(int[][] trainData)
        {
            int numCorrect = 0;
            int numWrong = 0;

            int[] xValues = new int[numInput];
            int target;
            int computed;

            for (int i = 0; i < trainData.Length; ++i)
            {
                Array.Copy(trainData[i], xValues, numInput); // get the inputs
                target = trainData[i][numInput]; // last value is target
                computed = ComputeY(xValues);

                if (computed == target)
                    ++numCorrect;
                else
                    ++numWrong;
            }
            return (numCorrect * 1.0) / (numCorrect + numWrong);
        }



        // -----------3
        static void MakeTrainTest(int[][] data, int seed, out int[][] trainData, out int[][] testData)
        {
            Random rnd = new Random(seed);
            int totRows = data.Length; // compute number of rows in each result
            int numTrainRows = (int)(totRows * 0.80);
            int numTestRows = totRows - numTrainRows;
            trainData = new int[numTrainRows][];
            testData = new int[numTestRows][];

            int[][] copy = new int[data.Length][]; // make a copy of data
            for (int i = 0; i < copy.Length; ++i)  // by reference to save space
                copy[i] = data[i];
            for (int i = 0; i < copy.Length; ++i) // scramble row order of copy
            {
                int r = rnd.Next(i, copy.Length);
                int[] tmp = copy[r];
                copy[r] = copy[i];
                copy[i] = tmp;
            }
            for (int i = 0; i < numTrainRows; ++i) // create training
                trainData[i] = copy[i];

            for (int i = 0; i < numTestRows; ++i) // create test
                testData[i] = copy[i + numTrainRows];
        } // MakeTrainTest


        // ------------------------------1
        static void ShowVector(double[] vector, int decimals, int valsPerRow, bool newLine)
        {
            for (int i = 0; i < vector.Length; ++i)
            {
                if (i % valsPerRow == 0) Console.WriteLine("");
                Console.Write(vector[i].ToString("F" + decimals).PadLeft(decimals + 4) + " ");
            }
            if (newLine == true) Console.WriteLine("");
        }

        static void ShowMatrix(int[][] matrix, int numRows, bool indices)
        {

			int pad = 3;

			Console.Write("[" + "Num".PadLeft(pad) + "] ");
			for (int j = 0; j < matrix[0].Length; ++j)
			{
				if (j < 10) pad = 2; else pad = 3;
				Console.Write(j.ToString().PadLeft(pad));
			}
			Console.WriteLine("");

            for (int i = 0; i < numRows; ++i)
            {
                if (indices == true)
					Console.Write("[" + i.ToString().PadLeft(pad) + "] ");
                for (int j = 0; j < matrix[i].Length; ++j)
                {
					if (j < 10) pad = 2; else pad = 3;
					Console.Write(matrix[i][j].ToString().PadLeft(pad));
                }
                Console.WriteLine("");
            }
            int lastIndex = matrix.Length - 1;
            if (indices == true)
				Console.Write("[" + lastIndex.ToString().PadLeft(pad) + "] ");
			for (int j = 0; j < matrix[lastIndex].Length; ++j)
			{
				if (j < 10) pad = 2; else pad = 3; 
				Console.Write(matrix[lastIndex][j].ToString().PadLeft(pad));
			}
            Console.WriteLine("\n");
        }

        static int[][] readFile(String fileName, int[][] data)
        {
            using (TextReader tr = new StreamReader(new FileStream(fileName, FileMode.Open)))
            {
                int numLine = 0;

                int setLength = 61; // the binary length of one line data


                String str;

				Console.WriteLine("Start Processing Data...");

                while ((str = tr.ReadLine()) != null)
                {
                    String[] oneLine = str.Split(',');

					for (int i = 0; i < oneLine.Length; i++)
					{
						Console.Write(oneLine[i].PadLeft(2) + " ");
					}
					Console.WriteLine();

                    data[numLine] = new int[setLength];

					data[numLine] = transData(oneLine, data[numLine]);

					numLine++;
                }
            }

			Console.WriteLine("Processing Data Completed.");

			return data;

        }


		static int[] transData(String[] oneLine, int[] data)
		{
			int setIter = 6; // # of Data type

			// descriptions see blow
			int speaker = 60;
			int instructor = 0; // 0 to 24
			int course = 25; // 25 to 50
			int semester = 51;
			int size = 52; // 52 to 56
			int score = 57; // 57 to 59



			for (int i = 0; i < setIter; i++)
			{
				//set (0): Native for 1; (1): Foreign for 2: n in data[][60]
				if (i == 0)
				{
					if (oneLine[i].Equals("1")) data[speaker] = 0;
					else if (oneLine[i].Equals("2")) data[speaker] = 1;
					else data[speaker] = -1;

				}

				// set 25 Course instructor n for (000...1...): n in data[][0] to data[][24]
				if (i == 1)
				{
					int indexInstructor = int.Parse(oneLine[i]) - 1 + instructor;
					for (int start = instructor; start < course; start++)
					{
						if (start == indexInstructor) data[start] = 1;
						else data[start] = 0;
					}

				}

				// set 26 Course: n in data[][25] to data[][50]
				if (i == 2)
				{
					int indexCourse = int.Parse(oneLine[i]) - 1 + course;

					for (int start = course; start < semester; start++)
					{
						if (start == indexCourse) data[start] = 1;
						else data[start] = 0;
					}
				}

				// set Semestrer: (0): Summer for 1; (1): Regular for 2; n in data[][51]
				if (i == 3)
				{
					if (oneLine[i].Equals("1")) data[semester] = 0;
					if (oneLine[i].Equals("2")) data[semester] = 1;
				}

				// set Course Size: (00001): n>=50; (00010): n>=40|n<50; (00100): n>=30|n<40; (01000): n>=20|n<30; (10000): n<20; n in data[][52] to data[][56]
				if (i == 4)
				{
					int indexSize = int.Parse(oneLine[i]) / 10;

					//Console.WriteLine(indexSize);

					if (indexSize >= 5) indexSize = 5;
					if (indexSize < 2) indexSize = 1;

					indexSize = indexSize - 1 + size;

					for (int start = size; start < score; start++)
					{
						if (start == indexSize) data[start] = 1;
						else data[start] = 0;
					}
				}

				// set Score: (001): high for 3; (010): medium for 2; (100): low for 1: n in data[][57] tp data[][59]
				if (i == 5)
				{
					int indexScore = int.Parse(oneLine[i]) - 1 + score;

					for (int start = score; start < speaker; start++)
					{
						if (start == indexScore) data[start] = 1;
						else data[start] = 0;
					}

				}
			}

			return data;
		}

		static void Main(String[] args)
		{ 
			// ---------------------------------------2
			int[][] data = new int[151][]; // 151 data in dataset

            data = readFile("tae.data" , data);

			Console.WriteLine("\nEncoding all data to binary, 'Native' = 0, 'Foreign' = 1");
			Console.WriteLine("Moving Type of TA (first data) to last column: see [60]");

			Console.WriteLine("\nFirst few lines and last of all data are: \n");

			ShowMatrix(data, 4, true);


			//-------3
			Console.WriteLine("\nSplitting data into 80% train" + " and 20% test matrices");
			int[][] trainData = null;
			int[][] testData = null;

			MakeTrainTest(data, 1, out trainData, out testData); // 21 for seed

			//Console.WriteLine("\nEncoding 'n' and '?' = 0, 'y' = 1, 'democrat' = 0, 'republican' = 1");
			Console.WriteLine("\nFirst few rows and last of training data are:\n");

			ShowMatrix(trainData, 3, true);

			//-----------5
			Console.WriteLine("\nBegin training using Winnow algorithm");

			int numInput = 60; // data analysis Num

			Winnow w = new Winnow(numInput, 1); // rndSeed = 0
			double[] weights = w.TrainWeights(trainData);
			Console.WriteLine("Training complete");

			Console.WriteLine("\nFinal model weights are:");

			ShowVector(weights, 4, 10, true);

			//-----6
			double trainAcc = w.Accuracy(trainData);
			double testAcc = w.Accuracy(testData);

			Console.WriteLine("\nPrediction accuracy on training data = " + trainAcc.ToString("F4"));
			Console.WriteLine("Prediction accuracy on test data = " + testAcc.ToString("F4"));

			//----7

			Console.WriteLine("\nPredicting Type of TA with data: '14,15,1,40,1'");

			String[] yays = new String[] {"-1" ,"14", "15", "1", "40", "1"}; // first num could be any number, add here to conform array format
			int[] test1 = new int[numInput+1];
			test1 = transData(yays, test1);

			int predicted = w.ComputeY(test1);

			if (predicted == 0)

				Console.WriteLine("Prediction is 'Native'");

			else

				Console.WriteLine("Prediction is 'Foreign'");



			Console.WriteLine("\nPredicting Type of TA with data '22,3,2,58,3'"); 

			String[] nays = new String[] {"-1", "22", "3", "2", "58", "3"}; // first num could be any number, add here to conform array format
			int[] test2 = new int[numInput+1];
			test2 = transData(nays, test2);


			int predicted2 = w.ComputeY(test2);

			if (predicted2 == 0)

				Console.WriteLine("Prediction is 'Native'");

			else

				Console.WriteLine("Prediction is 'Foreign'");



			Console.WriteLine("\nEnd Winnow demo\n");

			// for test
			//int[][] test = new int[2][];
			//test[0] = test1;
			//test[1] = test2;
			//ShowMatrix(test, 2, true);

		}

	} // Winnow

}
