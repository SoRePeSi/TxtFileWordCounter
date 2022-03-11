using System;
using System.IO;
using System.Collections.Generic;

namespace BookLog{
	class Program{
		// Directory with the files to count from:
		static string path = /*put path to file here; Example: @"C:Users\Owner\Documents\TxtFilesFolder"; */
		// Log file:
		static string logPath = /*put path to file here; Example: @"C:Users\Owner\Documents\ThisFolder\Log.txt"; */
		
		static int totalWords = 0;
			
		static void Main(string[] args){
			GetTotalWords();
			
			Console.WriteLine($"Word count: {totalWords} words");
			
			
			if(!File.Exists(logPath)){
				string firstLine = DateTime.Now.ToString().Split(" ")[0] + $" {totalWords - GetLastLogWords()} {totalWords}";
				
				AddNewLog(firstLine);
			}
			else{
				ClearEmptyLines();
				
				if(IsLatestLogWordDifferent()){
					string newLogLine = DateTime.Now.ToString().Split(" ")[0] + $" {totalWords - GetLastLogWords()} {totalWords}";
					
					if(IsLatestLogDateDifferent()){
						AddNewLog(newLogLine);
					}
					else{
						UpdateLatestLog(newLogLine);
					}
				}
				
				ClearEmptyLines();
			}
		}
		
		static void GetTotalWords(){
			string[] partsPath = Directory.GetFiles(path);
			
			for(int i=0; i<partsPath.Length; i++){
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
		
		static bool IsLatestLogDateDifferent(){
			string[] todayDate = DateTime.Now.ToString().Split(" ")[0].Split("/");
			
			if(Convert.ToInt32(todayDate[2]) > GetLastLogDate()[2]){	// Year
				return true;
			}
			else if(Convert.ToInt32(todayDate[1]) > GetLastLogDate()[1]){	// Month
				return true;
			}
			else if(Convert.ToInt32(todayDate[0]) > GetLastLogDate()[0]){	// Day
				return true;
			}
			
			return false;
		}
		
		static bool IsLatestLogWordDifferent(){
			if(totalWords != GetLastLogWords()){
				return true;
			}
			
			return false;
		}
		
		static int[] GetLastLogDate(){
			string[] logLines = File.ReadAllLines(logPath);
			string fullDate = logLines[logLines.Length-1].Split(" ")[0];
			string[] dayMonthYear = fullDate.Split("/");
			
			int day = Convert.ToInt32(dayMonthYear[0]);
			int month = Convert.ToInt32(dayMonthYear[1]);
			int year = Convert.ToInt32(dayMonthYear[2]);
			
			return new int[] {day, month, year};
		}
		
		static int GetLastLogWords(){
			string[] logLines = File.ReadAllLines(logPath);
			int lastLogWords = Convert.ToInt32(logLines[logLines.Length-1].Split(" ")[2]);
			
			return lastLogWords;
		}
	
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

		static void AddNewLog(string newLogLine){
			using (StreamWriter sw = File.AppendText(logPath)){
				sw.Write($"\n{newLogLine}");
			}
		}
		
		static void UpdateLatestLog(string newLogLine){
			string[] logLines = File.ReadAllLines(logPath);
			logLines[logLines.Length-1] = newLogLine;

			File.WriteAllLines(logPath, logLines);
		}
	}
}
