using System;
using System.IO;
using System.Collections.Generic;

namespace BookLog{
	class Program{
		// Directories to count from:
		static string[] folderPaths = /*put path to directories here; Example: {@"C:Users\Owner\Documents\TxtFilesFolder, @"C:Users\Owner\Documents\TxtOtherFolder}";*/;
		// Individual files to count from:
		static string[] filePaths = /*put path to individual files here; Example: {@"C:Users\Owner\Documents\TxtFilesFolder\file.txt, @"C:Users\Owner\Documents\TxtFilesFolder}";*/;
		// Log file:
		static string logPath = /*put path to file here; Example: @"C:Users\Owner\Documents\ThisFolder\Log.txt"; */;


		static int totalWords = 0;
		static int wordDifference = 0;
			
		static void Main(string[] args){
			GetTotalWords(folderPaths, filePaths);
			
			Console.WriteLine($"Word count: {totalWords} words");
			
			// Check if log file exists. Create one if it doesn't, procceed normally if it does:
			if(!File.Exists(logPath)){
				string firstLine = $"0/0/0 {wordDifference} {totalWords}";
				
				AddNewLog(firstLine);
			}
			else{
				ClearEmptyLines();
				
				// Change log if last logged word count is different:
				if(IsLatestLogWordDifferent()){
					// Add log line if last logged date is different is, change last line otherwise:
					if(IsLatestLogDateDifferent()){
						wordDifference = GetWordDifference("new");
						string newLogLine = DateTime.Now.ToString().Split(" ")[0] + $" {wordDifference} {totalWords}";
						
						AddNewLog(newLogLine);
					}
					else{
						wordDifference = GetWordDifference("previous");
						string newLogLine = DateTime.Now.ToString().Split(" ")[0] + $" {wordDifference} {totalWords}";
						
						UpdateLatestLog(newLogLine);
					}
				}
				
				ClearEmptyLines();
			}
		}
		
		// Count all words in specified folder. Disconsiders isolated punctuation for final count:
		static void GetTotalWords(string[] fldPaths, string[] fPaths){
			foreach(string path in fldPaths){
				string[] partsPath = Directory.GetFiles(path);
				
				for(int i=0; i<partsPath.Length; i++){
					if(Array.Exists(fPaths, element => element == partsPath[i])) continue;
					
					string[] lines = File.ReadAllLines(partsPath[i]);
					
					for(int j=0; j<lines.Length; j++){
						string[] words = lines[j].Split(" ");
						int amountOfPunctuation = 0;
						
						foreach(string k in words){
							if( (k.Length == 1) && (Char.IsPunctuation(k, 0)) ){
								amountOfPunctuation++;
							}
						}
						
						totalWords += words.Length - amountOfPunctuation;
					}
				}
			}
			foreach(string file in fPaths){
				string[] lines = File.ReadAllLines(file);
				
				for(int j=0; j<lines.Length; j++){
					string[] words = lines[j].Split(" ");
					int amountOfPunctuation = 0;
					
					foreach(string k in words){
						if( (k.Length == 1) && (Char.IsPunctuation(k, 0)) ){
							amountOfPunctuation++;
						}
					}
					
					totalWords += words.Length - amountOfPunctuation;
				}
			}
		}
		
		// Check if last logged date is different. Return true if it is, false otherwise:
		static bool IsLatestLogDateDifferent(){
			int[] todayDate = Array.ConvertAll(DateTime.Now.ToString().Split(" ")[0].Split("/"), s => int.Parse(s));
			int[] lastDate = GetLastLogDate();
			
			if(todayDate[2] > lastDate[2]){	// Year
				return true;
			}
			else if(todayDate[1] > lastDate[1]){	// Month
				return true;
			}
			else if(todayDate[0] > lastDate[0]){	// Day
				return true;
			}
			
			return false;
		}
		
		// Check if last logged word count is different. Return true if it is, false otherwise:
		static bool IsLatestLogWordDifferent(){
			if(totalWords != GetLastLogWords()){
				return true;
			}
			
			return false;
		}
		
		// Fetch last logged date and convert to int array:
		static int[] GetLastLogDate(){
			string[] logLines = File.ReadAllLines(logPath);
			string fullDate = logLines[logLines.Length-1].Split(" ")[0];
			string[] dayMonthYear = fullDate.Split("/");
			
			int day = Convert.ToInt32(dayMonthYear[0]);
			int month = Convert.ToInt32(dayMonthYear[1]);
			int year = Convert.ToInt32(dayMonthYear[2]);
			
			return new int[] {day, month, year};
		}
		
		// Fetch last logged word count and convert to int:
		static int GetLastLogWords(){
			string[] logLines = File.ReadAllLines(logPath);
			int lastLogWords = Convert.ToInt32(logLines[logLines.Length-1].Split(" ")[2]);
			
			return lastLogWords;
		}
	
		// Calculate writing progress based on last logged day that is not the day of use:
		static int GetWordDifference(string newOrPrevious){
			string[] logLines = File.ReadAllLines(logPath);
			int lastTotal = 0;
			
			if(newOrPrevious == "new"){
				lastTotal = Convert.ToInt32(logLines[logLines.Length-1].Split(" ")[2]);
			}
			else{
				lastTotal = Convert.ToInt32(logLines[logLines.Length-2].Split(" ")[2]);
			}
			
			int wordDifference = totalWords - lastTotal;
			
			return wordDifference;
		}
		
		// Check for empty lines in log. Erase them if they exist:
		static void ClearEmptyLines(){
			List<string> allLogLinesList = new List<string>();
			string[] allLogLines = File.ReadAllLines(logPath);
			
			for(int i=0; i<allLogLines.Length; i++){
				if( (String.IsNullOrWhiteSpace(allLogLines[i])) || (string.IsNullOrEmpty(allLogLines[i])) )
					continue;
				
				allLogLinesList.Add(allLogLines[i]);
			}
			
			allLogLines = new string[allLogLinesList.Count];
			
			for(int j=0; j<allLogLinesList.Count; j++){
				allLogLines[j] = allLogLinesList[j];
			}
			
			File.WriteAllLines(logPath, allLogLines);
		}

		// Add new line to log:
		static void AddNewLog(string newLogLine){
			using (StreamWriter sw = File.AppendText(logPath)){
				sw.Write($"\n{newLogLine}");
			}
		}
		
		// Chang latest line of log:
		static void UpdateLatestLog(string newLogLine){
			string[] logLines = File.ReadAllLines(logPath);
			logLines[logLines.Length-1] = newLogLine;

			File.WriteAllLines(logPath, logLines);
		}
	}
}
