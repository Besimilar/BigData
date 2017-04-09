package midTerm.HHW;

 import java.io.IOException;
import org.apache.hadoop.io.Writable; 
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.Mapper; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reporter;
 import org.apache.hadoop.io.Text; 
  
  
 public class GASelectMapper extends MapReduceBase implements Mapper<WritableComparable, Writable, Text, Text> { 
	 
	 // Select one chromosome, [0] for randomNum*totalFitness, [1] for isFound (-1 for notFound, or its index)
	 double[] rndFactor = new double[2]; 
	 
	 static int count = 0; // order to be processed
	 
	 {
			 rndFactor[0] = Chromosome.rand.nextDouble() * Chromosome.totalFitness;
			 //System.out.println(rndFactor[0]);
			 rndFactor[1] = -1;
			 if (count >= Chromosome.poolSize) count = 0;
	 }
	 
	 
	 public void map(WritableComparable key, Writable value, 
                   OutputCollector<Text, Text> output, Reporter reporter) throws IOException { 
  
	    // read the string
		String chromo = ((Text)value).toString(); 
		 
		// Index-binaryExpression-itsFitness-totalFitness
		String[] chromoDetail = chromo.split("\t");
		 	 
		// whether already find the Chromosome
	 
		 if (chromoDetail.length == 4) {
			 
			 double currTotFitness = Double.parseDouble(chromoDetail[3]);
				
			 if (rndFactor[1] == -1 && currTotFitness > rndFactor[0]) {
					 
			 // set isFound when find it
			 rndFactor[1] = Double.parseDouble(chromoDetail[0]) ;
							 
			 // map format:
			 // GroupIndex: ChromoIndex - BinaryExpression
			 Text index = new Text(new Integer((count++/2)).toString() + ":");
			 String toEmit = "";
			 toEmit += chromoDetail[0] + "\t" + chromoDetail[1];
							 
			 output.collect(index, new Text(toEmit));
					 
			 }
			 else {
			 
				 // map format:
				 // ChromoIndex - BinaryExpression - itsFitness
				 output.collect(new Text(chromoDetail[0]), new Text(chromoDetail[1] + "\t"
								+ chromoDetail[2]));  
			 }
		 
		 }
		 else {
			 
			 int index = chromo.indexOf(":"); 
			 String toEmit = chromo.substring(index+1).trim(); 
			 
			 /*output.collect(new Text(chromoDetail[0]), new Text(chromoDetail[1] + "\t"
					 			+ chromoDetail[2]));*/
			 output.collect(new Text(chromoDetail[0]), new Text(toEmit));
		 }
			

   } 
	 
	 
 } 
 