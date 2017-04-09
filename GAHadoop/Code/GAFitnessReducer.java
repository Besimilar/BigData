package midTerm.HHW;

 import java.io.IOException; 
 import java.util.Iterator; 
  
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reducer; 
 import org.apache.hadoop.mapred.Reporter; 
 import org.apache.hadoop.io.Text; 
  
 public class GAFitnessReducer extends MapReduceBase implements Reducer<WritableComparable, Text, Text, Text> { 
  
	 // accumulate scores
	 double AcScore = 0;
	 int AcOrder = 0;
	 
	 public void reduce(WritableComparable key, Iterator values, 
                      OutputCollector output, Reporter reporter) throws IOException { 
	    	 
	    	 if (!Chromosome.findSolution) { // if not find solution
	    		 
	    		 while (values.hasNext()) { 
	    			 
	    			 String currScore = ((Text)values.next()).toString(); 
	    		       
		    		 AcScore += Double.parseDouble(currScore);
		    		      
		    		 String toEmit = key + "\t" + currScore + "\t";
		    		   	
		    		 toEmit += (new Double(AcScore)).toString(); 
		    			 
		    		 output.collect(AcOrder++, new Text(toEmit)); 
		    				 
		    		 Chromosome.totalFitness = AcScore; // set AcScore for current generation
		    		
	    		 }
	    	 		
    	 	}
    	 	else { // if find solutions, output solutions

    	 		String rowData = ((Text)values.next()).toString();
    	 		if (rowData.contains("Solution")) 
    	 			output.collect(key, rowData);
    	 
    	 	}
	 }     
 } 
 