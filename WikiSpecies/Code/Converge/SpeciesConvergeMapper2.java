package assign5;
 import java.io.IOException; 
  
 import org.apache.hadoop.io.Writable; 
 import org.apache.hadoop.io.WritableComparable; 
 import org.apache.hadoop.mapred.MapReduceBase; 
 import org.apache.hadoop.mapred.Mapper; 
 import org.apache.hadoop.mapred.OutputCollector; 
 import org.apache.hadoop.mapred.Reporter; 
 import org.apache.hadoop.io.Text; 
  
  
 public class SpeciesConvergeMapper2 extends MapReduceBase implements Mapper<WritableComparable, Writable, Text, Text> { 
  
   public void map(WritableComparable key, Writable value, 
                   OutputCollector<Text, Text> output, Reporter reporter) throws IOException { 
  
     // get the current page 
     String data = ((Text)value).toString(); 
     int index = data.indexOf(":"); 
     
     // this line is really important to iteration.
     // it filters out all sink pages and remains only inlinks
     if (index == -1) { 
       return; 
     } 

     // split into title and ConvergeNum (tab or variable number of blank spaces)
     String toParse = data.substring(0, index).trim(); 
     String[] splits = toParse.split("\t"); 
     if(splits.length == 0) {
       splits = toParse.split(" ");
            if(splits.length == 0) {
               return;
            }
     }
     String pagetitle = splits[0].trim(); 
     String converge = splits[splits.length - 1].trim();
     
     // parse converge score
     double currScore = 0.0;
     try { 
        currScore = Double.parseDouble(converge); 
     } catch (Exception e) { 
        currScore = 0.0;
     } 

     // get number of outlinks
     /*data = data.substring(index+1); 
     String[] pages = data.split(" "); 
     int numoutlinks = 0;
     if (pages.length == 0) {
        numoutlinks = 1;
     } else {
       for (String page : pages) { 
         if(page.length() > 0) {
            numoutlinks = numoutlinks + 1;
         }
       } 
     }*/

     // collect each outlink, with the dampened PR of its inlink, and its inlink
     /*Text toEmit = new Text((new Double(.98 * currScore / numoutlinks)).toString()); 
     for (String page : pages) { 
       if(page.length() > 0) {
         output.collect(new Text(page), toEmit); 
         // output.collect(new Text(page), new  Text(" " + pagetitle)); // outlinks-inlinks, don't use this line
       }
     } */

     // collect the inlink with its dampening factor, and all outlinks
     // output.collect(new Text(pagetitle), new Text(".02")); 
     // output.collect(new Text(pagetitle), new Text(" inlink")); // for inlinks-outlinks
     
     // collect precious PR
     // output.collect(new Text(pagetitle), new Text(new Double(-1.0 * currScore).toString()));
     
     // collect Sum of Converge
     output.collect(new Text("Converge"), new Text(new Double(Math.pow(currScore, 2)).toString()));
   } 
 } 
 