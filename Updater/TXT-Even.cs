﻿#IMPORT System.Linq.dll
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TXTEven {
    public class Plain {
		bool? MSGOnly = true;
		
		
        string[] Script;
		Encoding Eco = Encoding.GetEncoding(932);
		bool BOOM = false;
        public Plain(byte[] Script) {
			if (Script[0] == 0xFF && Script[1] == 0xFE)
			{
				BOOM = true;
				byte[] narr = new byte[Script.Length-2];
				for (int i = 2; i < Script.Length; i++)
					narr[i-2] = Script[i];
				this.Script = Eco.GetString(narr).Replace("\r\n", "\n").Split('\n');
				return;
			}
            this.Script = Eco.GetString(Script).Replace("\r\n", "\n").Split('\n');
        }

        public string[] Import() {
			List<string> Lines = new List<string>();
			for (int i = 0; i < Script.Length; i++){
				string line = Script[i];
				if (line.StartsWith("s ") && MSGOnly != true){
					string Prefix = "s " + line.Split(' ')[1] + " ";
					line = line.Substring(Prefix.Length).Replace("*0A", "\n");
					if (string.IsNullOrEmpty(line))
						continue;
					foreach (string Line in line.Split('$'))
						Lines.Add(Line);
				} else if (line.StartsWith("mg ") && MSGOnly != false){
					string Str = string.Empty;
					while (Script[++i] != "@@"){
						Str += Script[i] + "\n";
					}
					Lines.Add(Str.Substring(0, Str.Length-1));
				}
			}
            return Lines.ToArray();
        }
		
		public void AppendArray<T>(ref T[] Array, T Value){
			T[] Output = new T[Array.Length+1];
			Array.CopyTo(Output, 0);
			Output[Array.Length] = Value;
			Array = Output;
		}

        public byte[] Export(string[] Text) {
			StringBuilder SB = new StringBuilder();
			for (int i = 0, x = 0; i < Script.Length; i++){
				string line = Script[i];
				if (line.StartsWith("s ") && MSGOnly != true){					
					string Prefix = "s " + line.Split(' ')[1] + " ";
					line = line.Substring(Prefix.Length);
					if (string.IsNullOrEmpty(line))
						continue;
					
					int Count = line.Split('$').Length;
					line = Prefix;
					for (int z = 0; z < Count; z++)
						line += Text[x++].Replace("\n", "*0A") + '$';
					SB.AppendLine(line.Substring(0, line.Length-1));
				} else if (line.StartsWith("mg ") && MSGOnly != false){
					SB.AppendLine(line);
					SB.AppendLine(Text[x++].Replace("\n", "\r\n"));
					SB.AppendLine("@@");
					while (Script[++i] != "@@")
						continue;
				} else {
					SB.AppendLine(line);					
				}
			}
			
			return Eco.GetBytes(SB.ToString());
        }

    }
}
