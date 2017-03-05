using System;
using System.IO;

namespace uciTALogistic
{
	using System;

	namespace LogisticLab
	{
		class MainClass
		{

			// 1-MakeAllData
			// generate observations
			static double[][] MakeAllData(String fileName, int numFeatures, int numRows, int seed)
			{
				Random rnd = new Random(seed);

				// numfeatures weights (bills)
				double[] weights = new double[numFeatures + 1]; // inc. b0
				for (int i = 0; i < weights.Length; ++i)
					weights[i] = 20.0 * rnd.NextDouble() - 10.0; // [-10.0 to +10.0]

				// numRows observations
				// Last column reserved for label, which is categorical binary
				double[][] result = new double[numRows][]; // allocate matrix
				for (int i = 0; i < numRows; ++i)
					result[i] = new double[numFeatures + 1]; // Y in last column


				// readFile from TA Data
				result = readFile(fileName, result);

				// generate random observations
				//for (int i = 0; i < numRows; ++i) // for each row
				//{
				//	double y = weights[0]; // the b0 
				//	for (int j = 0; j < numFeatures; ++j) // each feature / column except last
				//	{
				//		double x = 20.0 * rnd.NextDouble() - 10.0; // random X in [10.0, +10.0]
				//		result[i][j] = x; // store x

				//		double wx = x * weights[j + 1]; // weight * x 
				//		y += wx; // accumulate to get Y
				//				 // now add some noise
				//		y += numFeatures * rnd.NextDouble();
				//	}
				//	if (y > numFeatures) // because the noise was +, make it harder to be a 1
				//		result[i][numFeatures] = 1.0; // store y in last column
				//	else
				//		result[i][numFeatures] = 0.0;
				//}

				Console.WriteLine("Data generation weights:");
				ShowVector(weights, 2, 10, true);

				Console.WriteLine("\nFirst few lines of all data are (last column is the label): \n");
				ShowMatrix(result, 4, true);

				return result;
			}

			// method: readFile()

        	static double[][] readFile(String fileName, double[][] data)
			{
				using (TextReader tr = new StreamReader(new FileStream(fileName, FileMode.Open)))
				{
					int numLine = 0;

					int setLength = 17; // the binary length of one line data


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

						data[numLine] = new double[setLength];

						data[numLine] = transData(oneLine, data[numLine]);

						numLine++;
					}
				}

				Console.WriteLine("Processing Data Completed.");

				return data;

			}


			// transform Row Data
			static double[] transData(String[] oneLine, double[] data)
			{
				int setIter = 17; // # of Data type

				// descriptions see blow
				// int speaker = 60;
				// int instructor = 0; // 0 to 24
				// int course = 25; // 25 to 50
				// int semester = 51;
				// int size = 52; // 52 to 56
				// int score = 57; // 57 to 59



				for (int i = 0; i < setIter; i++)
				{
					if (i == 0)
					{
						if (oneLine[i].Equals("republican")) data[setIter-1] = 1; // republican = 1; democrat = 0;
						else if (oneLine[i].Equals("democrat")) data[setIter-1] = 0; // in the last column
						else data[setIter-1] = -1;

					}
					else if (oneLine[i].Equals("y")) data[i-1] = 1;
					else if (oneLine[i].Equals("n")) data[i-1] = 0;
					else data[i-1] = 0;

					
				}

				return data;
			}


			// 2-ShowVector and Show Matrix: utils
			public static void ShowVector(double[] vector, int decimals, int lineLen, bool newLine)
			{
				for (int i = 0; i < vector.Length; ++i)
				{
					if (i > 0 && i % lineLen == 0) Console.WriteLine("");
					if (vector[i] >= 0) Console.Write(" ");
					Console.Write(vector[i].ToString("F" + decimals).PadLeft(4) + "  ");
				}
				if (newLine == true)
					Console.WriteLine("");
			}

			//static void ShowMatrix(double[][] matrix, int numRows, int decimals, bool indices)
			//{
			//	//for (int i = 0; i < numRows; ++i)
			//	//{
			//	//	if (indices == true)
			//	//		Console.Write("[" + i.ToString().PadLeft(2) + "]   ");
			//	//	for (int j = 0; j < matrix[i].Length; ++j)
			//	//	{
			//	//		Console.Write(matrix[i][j].ToString("F" + decimals) + " ");
			//	//	}
			//	//	Console.WriteLine("");
			//	//}
			//	//int lastIndex = matrix.Length - 1;
			//	//if (indices == true)
			//	//	Console.Write("[" + lastIndex.ToString().PadLeft(2) + "]   ");
			//	//for (int j = 0; j < matrix[lastIndex].Length; ++j)
			//	//	Console.Write(matrix[lastIndex][j].ToString("F" + decimals) + " ");
			//	//Console.WriteLine("\n");
			//}


			static void ShowMatrix(double[][] matrix, int numRows, bool indices)
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



			// 3-Test Data
			static void MakeTrainTest(double[][] allData, int seed, out double[][] trainData, out double[][] testData)
			{
				Random rnd = new Random(seed);
				int totRows = allData.Length;
				int numTrainRows = (int)(totRows * 0.80); // 80% hard-coded
				int numTestRows = totRows - numTrainRows;
				trainData = new double[numTrainRows][];
				testData = new double[numTestRows][];

				double[][] copy = new double[allData.Length][]; // ref copy of all data
				for (int i = 0; i < copy.Length; ++i)
					copy[i] = allData[i];

				for (int i = 0; i < copy.Length; ++i) // scramble order
				{
					int r = rnd.Next(i, copy.Length); // use Fisher-Yates
					double[] tmp = copy[r];
					copy[r] = copy[i];
					copy[i] = tmp;
				}
				for (int i = 0; i < numTrainRows; ++i)
					trainData[i] = copy[i];

				for (int i = 0; i < numTestRows; ++i)
					testData[i] = copy[i + numTrainRows];
			}

			/// <summary>
			/// Main
			/// </summary>
			/// <param name="args">The command-line arguments.</param>
			public static void Main(string[] args)
			{
				// M1
				// int features = 16;
				int numFeatures = 16;
				int numRows = 435;
				int seed = 32;  // interesting seeds: 21, 32

				// generate observations
				Console.WriteLine("\nExtract " + numRows +
				                  " Voting data with " + numFeatures);
				double[][] allData = MakeAllData("house-votes-84.data.txt", numFeatures, numRows, seed);


				// M2
				// split into training and test datasets
				Console.WriteLine("Creating train (80%) and test (20%) matrices after scrambling observations order..");
				double[][] trainData;
				double[][] testData;
				MakeTrainTest(allData, 0, out trainData, out testData);
				Console.WriteLine("Done");
				Console.WriteLine("\nTraining data: \n");
				ShowMatrix(trainData, 4, true);
				Console.WriteLine("\nTest data: \n");
				ShowMatrix(testData, 3, true);


				Console.WriteLine("Enter to continue");
				Console.ReadLine();

				// M3
				// instantiate logistic binary classifier
				Console.WriteLine("Creating LR binary classifier..");
				LogisticClassifier lc = new LogisticClassifier(numFeatures, seed);

				// M4
				// train using no regularization
				int maxEpochs = 1000;
				Console.WriteLine("\nStarting training using no regularization..");
				double[] weights = lc.TrainWeights(trainData, maxEpochs, seed, 0.0, 0.0);

				Console.WriteLine("\nBest weights found:");
				ShowVector(weights, 2, weights.Length, true);

				double trainAccuracy = lc.Accuracy(trainData, weights);
				Console.WriteLine("Prediction accuracy on training data = " + trainAccuracy.ToString("F4"));

				double testAccuracy = lc.Accuracy(testData, weights);
				Console.WriteLine("Prediction accuracy on test data = " + testAccuracy.ToString("F4"));


				// M5
				// find L1 and L2
				Console.WriteLine("\nSeeking good L1 weight");
				double alpha1 = lc.FindGoodL1Weight(trainData, seed);
				Console.WriteLine("Good L1 weight = " + alpha1.ToString("F3"));

				Console.WriteLine("\nSeeking good L2 weight");
				double alpha2 = lc.FindGoodL2Weight(trainData, seed);
				Console.WriteLine("Good L2 weight = " + alpha2.ToString("F3"));


				// M6
				// train using L1 regularization
				Console.WriteLine("\nStarting training using L1 regularization, alpha1 = " + alpha1.ToString("F3"));
				weights = lc.TrainWeights(trainData, maxEpochs, seed, alpha1, 0.0);

				Console.WriteLine("\nBest weights found:");
				ShowVector(weights, 2, weights.Length, true);

				trainAccuracy = lc.Accuracy(trainData, weights);
				Console.WriteLine("Prediction accuracy on training data = " + trainAccuracy.ToString("F4"));

				testAccuracy = lc.Accuracy(testData, weights);
				Console.WriteLine("Prediction accuracy on test data = " + testAccuracy.ToString("F4"));

				// M7
				// train using L2 regularization
				Console.WriteLine("\nStarting training using L2 regularization, alpha2 = " + alpha2.ToString("F3"));
				weights = lc.TrainWeights(trainData, maxEpochs, seed, 0.0, alpha2);

				Console.WriteLine("\nBest weights found:");
				ShowVector(weights, 2, weights.Length, true);

				trainAccuracy = lc.Accuracy(trainData, weights);
				Console.WriteLine("Prediction accuracy on training data = " + trainAccuracy.ToString("F4"));

				testAccuracy = lc.Accuracy(testData, weights);
				Console.WriteLine("Prediction accuracy on test data = " + testAccuracy.ToString("F4"));

				Console.WriteLine("\nEnd Regularization demo\n");
				Console.ReadLine();

			}






			// 4-Classifier
			public class LogisticClassifier
			{
				private int numFeatures; // number of independent variables aka features
										 // private double[] weights; // b0 = constant
				private Random rnd;

				private int seed;

				public LogisticClassifier(int numFeatures, int seed)
				{
					this.numFeatures = numFeatures; // number features/predictors
													//this.weights = new double[numFeatures + 1]; // [0] = b0 constant
					this.rnd = new Random(seed); // seed could be a parmeter to ctor

					this.seed = seed;
				}

				public double FindGoodL1Weight(double[][] trainData, int seed)
				{
					double result = 0.0;
					double bestErr = double.MaxValue;
					double currErr = double.MaxValue;

					// why choose these candidates???
					double[] candidates = new double[] { 0.000, 0.001, 0.005, 0.010, 0.020, 0.050, 0.100, 0.150 };
					int maxEpochs = 1000;

					LogisticClassifier c = new LogisticClassifier(this.numFeatures, this.seed);

					for (int trial = 0; trial < candidates.Length; ++trial)
					{
						double alpha1 = candidates[trial];
						double[] wts = c.TrainWeights(trainData, maxEpochs, seed, alpha1, 0.0);
						currErr = Error(trainData, wts, 0.0, 0.0);
						if (currErr < bestErr)
						{
							bestErr = currErr;
							result = candidates[trial];
						}
					}
					return result;
				}

				public double FindGoodL2Weight(double[][] trainData, int seed)
				{
					double result = 0.0;
					double bestErr = double.MaxValue;
					double currErr = double.MaxValue;
					double[] candidates = new double[] { 0.000, 0.001, 0.005, 0.010, 0.020, 0.050, 0.100, 0.150 };
					int maxEpochs = 1000;

					LogisticClassifier c = new LogisticClassifier(this.numFeatures, this.seed);

					for (int trial = 0; trial < candidates.Length; ++trial)
					{
						double alpha2 = candidates[trial];
						double[] wts = c.TrainWeights(trainData, maxEpochs, seed, 0.0, alpha2);
						currErr = Error(trainData, wts, 0.0, 0.0);
						if (currErr < bestErr)
						{
							bestErr = currErr;
							result = candidates[trial];
						}
					}
					return result;
				}

				public double[] TrainWeights(double[][] trainData, int maxEpochs, int seed, double alpha1, double alpha2)
				{
					// use PSO. particle position == LR weights

					// ************* independent var + 1
					int numParticles = 17;
					double probDeath = 0.005;
					int dim = this.numFeatures + 1; // need one wt for each feature, plus the b0 constant

					Random rnd = new Random(seed);

					int epoch = 0;
					double minX = -10.0; // for each weight. assumes data has been normalized about 0
					double maxX = 10.0;
					double w = 0.729; // inertia weight
					double c1 = 1.49445; // cognitive/local weight
					double c2 = 1.49445; // social/global weight
					double r1, r2; // cognitive and social randomizations

					Particle[] swarm = new Particle[numParticles];
					// best solution found by any particle in the swarm. implicit initialization to all 0.0
					double[] bestSwarmPosition = new double[dim];
					double bestSwarmError = double.MaxValue; // smaller values better

					// initialize each Particle in the swarm with random positions and velocities
					for (int i = 0; i < swarm.Length; ++i)
					{
						double[] randomPosition = new double[dim];
						for (int j = 0; j < randomPosition.Length; ++j)
							randomPosition[j] = (maxX - minX) * rnd.NextDouble() + minX;

						// randomPosition is a set of weights
						double error = Error(trainData, randomPosition, alpha1, alpha2);
						double[] randomVelocity = new double[dim];
						for (int j = 0; j < randomVelocity.Length; ++j)
						{
							double lo = 0.1 * minX;
							double hi = 0.1 * maxX;
							randomVelocity[j] = (hi - lo) * rnd.NextDouble() + lo;
						}
						swarm[i] = new Particle(randomPosition, error, randomVelocity,
							randomPosition, error); // last two are best-position and best-error

						// does current Particle have global best position?
						// best position for the particle is the one that's closest to the label (Y)
						if (swarm[i].error < bestSwarmError)
						{
							bestSwarmError = swarm[i].error;
							swarm[i].position.CopyTo(bestSwarmPosition, 0);
						}
					}

					// main PSO algorithm
					int[] sequence = new int[numParticles]; // process particles in random order
					for (int i = 0; i < sequence.Length; ++i)
						sequence[i] = i;

					while (epoch < maxEpochs)
					{
						double[] newVelocity = new double[dim]; // step 1
						double[] newPosition = new double[dim]; // step 2
						double newError; // step 3

						Shuffle(sequence); // move particles in random sequence

						for (int pi = 0; pi < swarm.Length; ++pi) // each Particle (index)
						{
							int i = sequence[pi];
							Particle currP = swarm[i]; // for coding convenience

							// 1. compute new velocity
							for (int j = 0; j < currP.velocity.Length; ++j) // each x value of the velocity
							{
								r1 = rnd.NextDouble();
								r2 = rnd.NextDouble();

								// velocity depends on old velocity, best position of particle, and 
								// best position of any particle
								newVelocity[j] = (w * currP.velocity[j]) +
									(c1 * r1 * (currP.bestPosition[j] - currP.position[j])) +
									(c2 * r2 * (bestSwarmPosition[j] - currP.position[j]));
							}

							newVelocity.CopyTo(currP.velocity, 0);

							// 2. use new velocity to compute new position
							for (int j = 0; j < currP.position.Length; ++j)
							{
								newPosition[j] = currP.position[j] + newVelocity[j];  // compute new position
								if (newPosition[j] < minX) // keep in range
									newPosition[j] = minX;
								else if (newPosition[j] > maxX)
									newPosition[j] = maxX;
							}

							newPosition.CopyTo(currP.position, 0);

							// 3. use new position to compute new error
							newError = Error(trainData, newPosition, alpha1, alpha2);
							currP.error = newError;

							if (newError < currP.bestError) // new particle best?
							{
								newPosition.CopyTo(currP.bestPosition, 0);
								currP.bestError = newError;
							}

							if (newError < bestSwarmError) // new swarm best?
							{
								newPosition.CopyTo(bestSwarmPosition, 0);
								bestSwarmError = newError;
							}

							// 4. optional: does curr particle die?
							double die = rnd.NextDouble();
							if (die < probDeath)
							{
								// new position, leave velocity, update error
								for (int j = 0; j < currP.position.Length; ++j)
									currP.position[j] = (maxX - minX) * rnd.NextDouble() + minX;
								currP.error = Error(trainData, currP.position, alpha1, alpha2);
								currP.position.CopyTo(currP.bestPosition, 0);
								currP.bestError = currP.error;

								if (currP.error < bestSwarmError) // swarm best by chance?
								{
									bestSwarmError = currP.error;
									currP.position.CopyTo(bestSwarmPosition, 0);
								}
							}

						} // each Particle
						++epoch;
					} // while

					double[] retResult = new double[dim];
					Array.Copy(bestSwarmPosition, retResult, retResult.Length);
					return retResult;
				} // TrainWeights

				private void Shuffle(int[] sequence)
				{
					for (int i = 0; i < sequence.Length; ++i)
					{
						int r = rnd.Next(i, sequence.Length);
						int tmp = sequence[r];
						sequence[r] = sequence[i];
						sequence[i] = tmp;
					}
				}

				public double Error(double[][] trainData, double[] weights, double alpha1, double alpha2)
				{
					// mean squared error using supplied weights
					// L1 regularization adds the sum of the absolute values of the weights
					// L2 regularization adds the sqrt of sum of squared values

					int yIndex = trainData[0].Length - 1; // y-value (0/1) is last column
					double sumSquaredError = 0.0;
					for (int i = 0; i < trainData.Length; ++i) // each data
					{
						double computed = ComputeY(trainData[i], weights);
						double desired = trainData[i][yIndex]; // ex: 0.0 or 1.0
						sumSquaredError += (computed - desired) * (computed - desired);
					}

					double sumAbsVals = 0.0; // L1 penalty
					for (int i = 0; i < weights.Length; ++i)
						sumAbsVals += Math.Abs(weights[i]);

					double sumSquaredVals = 0.0; // L2 penalty
					for (int i = 0; i < weights.Length; ++i)
						sumSquaredVals += (weights[i] * weights[i]);
					//double rootSum = Math.Sqrt(sumSquaredVals);

					return (sumSquaredError / trainData.Length) +
						(alpha1 * sumAbsVals) +
						(alpha2 * sumSquaredVals);
				}

				public double ComputeY(double[] dataItem, double[] weights)
				{
					double z = 0.0;

					z += weights[0]; // the b0 constant
					for (int i = 0; i < weights.Length - 1; ++i) // data might include Y
						z += (weights[i + 1] * dataItem[i]); // skip first weight

					return 1.0 / (1.0 + Math.Exp(-z));
				}

				public int ComputeDependent(double[] dataItem, double[] weights)
				{
					double sum = ComputeY(dataItem, weights);

					if (sum <= 0.5)
						return 0;
					else
						return 1;
				}

				public double Accuracy(double[][] trainData, double[] weights)
				{
					int numCorrect = 0;
					int numWrong = 0;
					int yIndex = trainData[0].Length - 1;
					for (int i = 0; i < trainData.Length; ++i)
					{
						double computed = ComputeDependent(trainData[i], weights); // implicit cast
						double desired = trainData[i][yIndex]; // 0.0 or 1.0

						double epsilon = 0.0000000001;
						if (Math.Abs(computed - desired) < epsilon)
							++numCorrect;
						else
							++numWrong;
					}
					return (numCorrect * 1.0) / (numWrong + numCorrect);
				}

				public class Particle // for PSO training
				{
					public double[] position; // equivalent to weights
					public double error; // measure of fitness
					public double[] velocity; // determines new position
					public double[] bestPosition; // best found by this Particle
					public double bestError;

					public Particle(double[] position, double error, double[] velocity,
					double[] bestPosition, double bestError)
					{
						this.position = new double[position.Length];
						position.CopyTo(this.position, 0);
						this.error = error;
						this.velocity = new double[velocity.Length];
						velocity.CopyTo(this.velocity, 0);
						this.bestPosition = new double[bestPosition.Length];
						bestPosition.CopyTo(this.bestPosition, 0);
						this.bestError = bestError;
					}
				} // (nested) class Particle
			} // LogisticClassifier





		}
	}

}