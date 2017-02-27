package assign5;

 import java.io.IOException; 
 import java.util.Iterator; 
  
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reducer; 
 import org.apache.hadoop.mapred.Reporter; 
 import org.apache.hadoop.io.Text; 
  
 public class SpeciesConvergeReducer2 extends MapReduceBase implements Reducer<WritableComparable, Text, Text, Text> { 
  
   public void reduce(WritableComparable key, Iterator values, 
                      OutputCollector output, Reporter reporter) throws IOException { 
     double converge = 0; 
     String inlinks = ""; 
     //Boolean reduced = false;
  
     while (values.hasNext()) { 
       String curr = ((Text)values.next()).toString(); 
       
       converge += Double.parseDouble(curr);
      /* int colon = curr.indexOf(":"); 
       int space = curr.indexOf(" "); 

       if ((colon > -1)) { // seems no need, but strange here, in map output
    	   						// there is no any colon!!!  
         String presScore = curr.substring(0, colon); 
         try { 
           converge += Double.parseDouble(presScore); 
           inlinks += curr.substring(colon + 1); 
           continue; 
         } catch (Exception e) { 
           ; 
         } 
       } */

       /*if (space > -1) { 
         inlinks += curr; 
       } else { 
         score += Double.parseDouble(curr); 
       } 
       */
       //reduced = true;
       
     } 

     String toEmit = new Double(converge).toString(); 
     //if (reduced){
	 	/*if (inlinks.length() > 0) { 
    	       toEmit = (new Double(Math.pow(score, 2))).toString() + ":" + inlinks; 
    	    } else { 
    	       toEmit = (new Double(score)).toString(); 
    	    } */
	 	
	 	// collect Sum of converge: "Converge" + result;
	 	output.collect(key, new Text(toEmit)); 
     //}
     
    
   } 
 } 
 