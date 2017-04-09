package midTerm.HHW;

import java.io.IOException;
import java.util.logging.Level;
import java.util.logging.Logger;

import org.apache.hadoop.fs.FileSystem;
import org.apache.hadoop.fs.Path;
import org.apache.hadoop.io.LongWritable;
import org.apache.hadoop.io.Text;
import org.apache.hadoop.mapred.FileInputFormat;
import org.apache.hadoop.mapred.FileOutputFormat;
import org.apache.hadoop.mapred.JobClient;
import org.apache.hadoop.mapred.JobConf;

public class GADriver {
	
	public static void main(String[] args)  throws Exception
	{ 
		
		String input = args[0];
		String output = args[0];
		int target = Integer.parseInt(args[1]);
		Chromosome.target = target;
		int count = 0;
		
		if (args.length < 2) { 
			System.out.println("Usage: GA-Hadoop <input path> <target int>"); 
			System.exit(0); 
		} 
		
		
		while (!Chromosome.findSolution && count++<2000) { // set max generations
		
		// Job1 for Fitness Calculate
		// If a solution is found, output the solution.
		JobClient client = new JobClient(); 
		JobConf conf = new JobConf(GADriver.class); 
		conf.setJobName("Fitness Calculation"); 
		  
		conf.setNumReduceTasks(1); 
		  
		conf.setOutputKeyClass(Text.class); 
		conf.setOutputValueClass(Text.class); 
		
		conf.setMapperClass(GAFitnessMapper.class); 
		conf.setReducerClass(GAFitnessReducer.class); 
		
		input = output;
		output = "Solutions";  
		Path outPath = new Path(output);
		FileInputFormat.addInputPath(conf, new Path(input));
		FileOutputFormat.setOutputPath(conf, outPath);

		FileSystem fs = FileSystem.get(outPath.toUri(), conf);
		if (fs.exists(outPath)) {
			fs.delete(outPath, true);
		}

		client.setConf(conf);
		try {
			JobClient.runJob(conf);
		} catch (IOException ex) {
			Logger.getLogger(GADriver.class.getName()).log(
					Level.SEVERE, null, ex);
		}
		
	
	    //Job2 Chromosome Select
		 
		if (!Chromosome.findSolution) {
			
			for (int i =0 ; i < Chromosome.poolSize; i++) {
				 
				input = output;
			    //output = "Selected_from_"+ count++ + "gen";
				output = "SelectedChromos+" + i;
			     
				JobConf conf2 = new JobConf(GADriver.class); 
				conf2.setJobName("Chromosome Select"); 
				  
				conf2.setNumReduceTasks(1); 
				  
				conf2.setOutputKeyClass(Text.class); 
				conf2.setOutputValueClass(Text.class); 
				  
				FileInputFormat.setInputPaths(conf2, new Path(input));
				FileOutputFormat.setOutputPath(conf2, new Path(output));
				
				outPath = new Path(output);
				FileSystem fs2 = FileSystem.get(outPath.toUri(), conf2);
				if (fs2.exists(outPath)) {
					fs2.delete(outPath, true);
				}
			
				conf2.setMapperClass(GASelectMapper.class); 
				conf2.setReducerClass(GASelectReducer.class);
				 
				conf2.setOutputKeyComparatorClass(LongWritable.Comparator.class);
			        

			    client.setConf(conf2);
				try {
					JobClient.runJob(conf2);
				} catch (IOException ex) {
					Logger.getLogger(GADriver.class.getName()).log(
							Level.SEVERE, null, ex);
				}
				
			 }
			
		}
		

		//Job3 Reproduce
	    
		if (!Chromosome.findSolution) {
			
			input = output;
		    output = "ProducedChromos";
			
		    JobConf conf3 = new JobConf(GADriver.class); 
			conf3.setJobName("Chromosome Reproduce"); 
			  
			conf3.setNumReduceTasks(1); 
			  
			conf3.setOutputKeyClass(Text.class); 
			conf3.setOutputValueClass(Text.class); 
		
			FileInputFormat.setInputPaths(conf3, new Path(input));
			FileOutputFormat.setOutputPath(conf3, new Path(output));
			
			outPath = new Path(output);
			FileSystem fs3 = FileSystem.get(outPath.toUri(), conf3);
			if (fs3.exists(outPath)) {
				fs3.delete(outPath, true);
			}
		
			conf3.setMapperClass(GAReproduceMapper.class); 
			conf3.setReducerClass(GAReproduceReducer.class);
			 
			client.setConf(conf3);
			try {
				JobClient.runJob(conf3);
			} catch (IOException ex) {
				Logger.getLogger(GADriver.class.getName()).log(
						Level.SEVERE, null, ex);
			}
			
		}
		
		
		
		}
		
	}
	
}
