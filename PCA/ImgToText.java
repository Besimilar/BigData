package tools;

import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;
import weka.attributeSelection.PrincipalComponents;

import javax.imageio.ImageIO;

public class ImgToText {

	public static void main(String[] args) {
		
		// read image
		File imgFile = new File("Cat.jpg"); // 14kb
		
		try {
			BufferedImage img = ImageIO.read(imgFile);
			
			int height = img.getHeight(); // height of image
			int width = img.getWidth(); // width of image
			
			System.out.println(height);
			System.out.println(width);
			
			// Matrix for image
			int[][] imgMatrix = new int[height][width];
			
			for (int i = 0; i<height; i++) {
				
				for (int j = 0; j<width; j++) {
					
					// getRGB (x,y)
					// x: width
					// i: height
					imgMatrix[i][j] = img.getRGB(j, i);
					//System.out.print(i + " " + j);
					//System.out.println();
					//System.out.println(imgMatrix[i][j]);
					
					// extract red, green, blue
					int r = (imgMatrix[i][j] >> 16) & 0x000000FF;
					int g = (imgMatrix[i][j] >> 8) & 0x000000FF;
					int b = (imgMatrix[i][j]) & 0x000000FF;
					
					// System.out.println(j + "-" + i + ": " + r + " " + g + " " + b);
					
					// transform to grey
					// RGB to Gray: r * 0.299 + 0.587 * g + 0.114 * b 
					int grey = (r * 299 + 587 * g + 114 * b + 500)/1000; // 500 for round
				}
			}
			
			
			
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
		
		// test PCA
		// PrincipalComponents pca = new PrincipalComponents();
		
		
	}

}
