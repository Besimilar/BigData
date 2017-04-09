package midTerm.HHW;

 import java.io.IOException; 
 import java.util.Iterator; 
  
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reducer; 
 import org.apache.hadoop.mapred.Reporter; 
 import org.apache.hadoop.io.Text; 
  
 public class GASelectReducer extends MapReduceBase implements Reducer<WritableComparable, Text, Text, Text> { 
  
	 double AcScore = 0;
	 
	 public void reduce(WritableComparable key, Iterator values, 
                      OutputCollector output, Reporter reporter) throws IOException { 
		 
		 String toEmit = "";
		 
		 while (values.hasNext()) {
			 
			 String currChromo = ((Text)values.next()).toString();
			 
			 if (key.toString().contains(":")) { // processed Chromos
				 
				 String[] currChromoDetail = currChromo.split("\t");
				 
				 if (currChromoDetail.length == 2) {
					 
					 // map format:
					 // GroupIndex: - ChromoIndex - BinaryExpression
					 toEmit += currChromoDetail[0] + "\t" + currChromoDetail[1] + "\t";
				 }
				 else toEmit = currChromo;
				 
			 }
			 else {
				 
				// map format:
			    // ChromoIndex(Key) - BinaryExpresson - itsFitness
			    String[] currChromoDetail = currChromo.split("\t");
			       
			    String currScore = currChromoDetail[1]; // read Fitness
 		       
	    		 	AcScore += Double.parseDouble(currScore);
	    		      
	    		    toEmit = currChromoDetail[0] + "\t" + currScore + "\t";
	    		   	
	    		 	toEmit += (new Double(AcScore)).toString(); 
	    			 
	    		 	output.collect(key, new Text(toEmit)); 
	    				 
	    		 	Chromosome.totalFitness = AcScore; // set AcScore for current generation
	    		
			 }
	    	 
	     }
		 
		 if (key.toString().contains(":")) {
			 output.collect(key, toEmit);
		 }
		 
   } 
 } 
 