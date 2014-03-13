using UnityEngine;
using System.Collections;
using System.IO;
using System.Net;
using System.Diagnostics; 

// This is more a sample of possibilities to connect Unity3D to popular chess servers...
// Of course, everything may be improved.
// Present version just goes to freechess.org and plays a game offered by others (minimum 10minutes)
// Telnet session is more reliable then direct sockets programming by non-professional, and it's slower because of
// long way for data via multiple step-by-step  interfaces... Unity3d-FicsCall.exe-PuttyTelnet-Unix server of Fics.org

public class Ch_socket : MonoBehaviour {
		

	string login="guest";
	string password="anonymous";
	
	int status=0;											// Connection statuss
	
	bool FicsAccessible=false;
	
	string FicsConnFile="";
	string FicsConnPath="";

	Process pFicsConn= new Process();
	float FicsConnTime=0;										// to pause Fics socket connection trying before Putty telnet started...
	float time_difference=0.2f;								// default local web-connection update interval...
	float lastUpdate=0;											// seconds after last update
	float wantPlayTime=0;										// to escape too long waiting for game accepts
	
	string MoveMade="";
	
	bool autoCmds=true;									// automatic mode on/off

	// Use this for initialization
	void Start ()
		{
		FicsAccessible=false;					// Fics just for Windows Editor or Windows Standalone... (no WebPlayer)
		if(((Application.platform.ToString()).IndexOf("Windows")>=0) && ((Application.platform.ToString()).IndexOf("Web")<0))
			{
			string editpath=Application.dataPath;
			if( (Application.platform.ToString()).IndexOf("WindowsEditor")<0 ) editpath = "";
			else editpath = editpath.Substring(0,editpath.LastIndexOf("Assets"))+"standalone\\Windows\\";
 
			FicsConnFile = editpath + "FICSconn\\FicsConn.exe";
				
			FicsAccessible = System.IO.File.Exists(FicsConnFile);			// if file exists...
			if(FicsAccessible)
				{
				int at=FicsConnFile.LastIndexOf("\\");
				FicsConnPath=((at<0) ? "" : FicsConnFile.Substring(0,at+1));
				pFicsConn=null;
				}
			}
		Let3DknowAccess();
		}
		
	void Let3DknowAccess()
		{
		(GameObject.Find("Script2")).SendMessage("TcpAccess", (FicsAccessible?"YES":"NO"));
		if(!FicsAccessible) status = -1;
		}
	
	void ConnectServer(string parm)			// Start local web & telnet program (1.step)
		{
		if(FicsAccessible)
			{
			(GameObject.Find("Script2")).SendMessage("TcpAddMessage", "Trying to connect and start a game"+"\n" +
				"Press Esc to turn on command mode (login keyboard, fics commands, help, etc."+"\n\n");
				
			pFicsConn= new Process();
			pFicsConn.StartInfo.WorkingDirectory = FicsConnPath;	// !!! just make it as a comment to avoid build-compile error for Web platform !!!
            pFicsConn.StartInfo.CreateNoWindow = true;
			pFicsConn.StartInfo.FileName = FicsConnFile;		// Call FicsConn.exe to establish local sockets server+telnet to FICS...
            pFicsConn.StartInfo.Arguments = "";
			pFicsConn.StartInfo.UseShellExecute = false;
			pFicsConn.Start();
			
			if(pFicsConn==null) { FicsAccessible=false; Let3DknowAccess(); }
			else { FicsConnTime=Time.time;  status=1; Status2MainScript(); }			// Process started...
			
			}
		}
	
		void ConnectServer2()							// Try to establish connection to local web (2.step) with program started on step1
		{
		if(FicsAccessible)
			{
			//TcpClient Client = new TcpClient("127.0.0.1", 5050);		// Local on 5050 port... FicsConn.exe
			status = 2;			// Ok, use sockets later...
			}
		}
			
		
	void ReadFromServer()							// Buffered get from local web... Fast enough. Don't like reading bytes from sockets...
		{
		if(FicsAccessible && (status>1))
		{
		string buffer="";
		HttpWebRequest request;
		HttpWebResponse response;
		StreamReader reader;
		request = WebRequest.Create("http://localhost:5050/IN_BUFFER.txt") as HttpWebRequest;
		using (response = request.GetResponse() as HttpWebResponse)  
			{  
			reader = new StreamReader(response.GetResponseStream());  
			buffer=reader.ReadToEnd(); 
			reader.Close();
			}
	
		if((response==null) || (buffer.IndexOf("[PUTTY-FICS-IN-BUFFER-OK]")<0))
			{
			// do nothing, next request can be successfull...(especially in the middle of a game)		
			// FicsAccessible=false; Let3DknowAccess();
			}
			
		string sLine="";						//Separate lines...
		for(; buffer.Length>0; )
			{
			int a1=buffer.IndexOf("\n");
			if(a1<0) { sLine=buffer; buffer=""; }
			else { sLine=buffer.Substring(0,a1); buffer=buffer.Substring(a1+1); }
			if( (sLine.Length>0) && (sLine.IndexOf("[PUTTY-FICS-IN-BUFFER-OK]")<0))
				{
				ProcessLineIN(sLine); 
				(GameObject.Find("Script2")).SendMessage("TcpAddMessage", sLine+"\n");
				}
			}
		}
		}
		
				// analyse incomming data from stream...
		void ProcessLineIN(string sLine)
		{
			string play_cmd;
			string real_nick;
			
			if(autoCmds && (status==2) && ((sLine.ToUpper()).IndexOf("LOGIN")>=0)) status=3;
			if(autoCmds && (status==4) && ((sLine.ToUpper()).IndexOf("PASSWORD")>=0)) status=6;
			if((status==5) && (sLine.IndexOf("Press return to enter the server")>=0))
				{
				real_nick=sLine.Substring(sLine.IndexOf("\"")+1);
				real_nick=real_nick.Substring(0,real_nick.IndexOf("\""));
				login=real_nick;				// the right login name...	
				status=6;
				}
				
			if(status==7)
				{

					// hate chats & dumb messages...
				SendLinesOUT("set silence 1");	// Set silence mode...	
				SendLinesOUT("set shout 0");
				SendLinesOUT("set kibitz 0");	
				SendLinesOUT("set tell 0");	
				SendLinesOUT("set chanoff 1");	
				SendLinesOUT("set echo 1");	
					
				//	There is no big reason in style12, because connection is buffered and fics sends all the board in single bunch of bytes...
				//	This version doesn't resume after lost connection, that is worse then other lacks of code...
				//SendLinesOUT("set style 12");			// Set single-line information about game...	
				status=9;
				}
			
			if((status==11) && (sLine.IndexOf("seek is not available")>=0)) status=9;			// game not possible...
			if((status==11)	&& ((sLine.ToUpper()).IndexOf("CREATING")>=0)) status=20;			// game is possible...		
			
						// after successfully logged in, catch any offered 10/15/20 min. blitz/standard game just to play
			if( ((sLine.ToUpper()).IndexOf("PLAY")>=0) &&
				( ((sLine.ToUpper()).IndexOf("BLITZ")>=0) || ((sLine.ToUpper()).IndexOf("STANDARD")>=0) ) &&
						(((sLine.ToUpper()).IndexOf("SEEKING 10 ")>=0) ||
						((sLine.ToUpper()).IndexOf("SEEKING 15 ")>=0) ||
						((sLine.ToUpper()).IndexOf("SEEKING 20 ")>=0)) )
				{
				if((status==9) && autoCmds)
					{
					play_cmd=sLine.Substring(sLine.IndexOf("\"")+1);
					play_cmd=play_cmd.Substring(0,play_cmd.IndexOf("\""));	
					status=10;						// Ready to play...
					SendLinesOUT(play_cmd);	// try to process this game...
					}
				else if(status<9) status=9;		// when manual login performed...
				}
			if(status==20)
				{
												// Should send information about our color...
				if((sLine.IndexOf(" vs. ")>=0) && (sLine.IndexOf(login)>=0))
					{
					(GameObject.Find("Script2")).SendMessage("TcpOurColor",((sLine.IndexOf(login+")")>=0) ? "black" : "white"));
					status=21;
					}
				}
		}
				// send data according status...
		void SendLinesOUT(string txt)
		{
			if((txt.Length>5) && (txt.Substring(0,5)=="CMD: "))
				{
				string cmd=txt.Substring(5);
				SendToServer(cmd);		// cmd mode...
				if(status==2)
					{
					login=cmd.Replace(" ","");		// the right login name...	
					status=4;
					}
				}
			else if(status==3)			// login...
				{
				if(login.IndexOf("guest")>=0)
					{	
					password="";		// just enter...
					status=5; 
					}
				else status=4;
					
				SendToServer(login);
				}
			else if(status==6)		// password...
				{
				SendToServer(password); status=7;
				}
			else if(status==7)		// at first, goes configuration of fics settings...
				{
				SendToServer(txt);
				}
			else if(status==10)	// want to play a game found in incomming seek...
				{
				SendToServer(txt); status=11;
				}
			else if(status==21)		// when game goes on, can press buttons...
				{
				if(((txt.Length>=6) && (txt=="resign")) || ((txt.Length>=4) && (txt=="draw")) )
					{
					SendToServer(txt);
					}
				else if(MoveMade.Length>0)		// After mousedrag-piece-movement on board here comes the last move that should be sent to the server... 
					{
					SendToServer(MoveMade); 			// when move arrived...
					MoveMade="";
					}
				}
		}

	void SendToServer(string tLine)							// Buffered put to local web... all the string goes in one url-type post request...
		{
		if(FicsAccessible && (status>1))
		{
		string buffer="";
		HttpWebRequest request;
		HttpWebResponse response;
		StreamReader reader;
		request = WebRequest.Create("http://localhost:5050/OUT_BUFFER.txt?OUT_BUFFER="+tLine+"\n") as HttpWebRequest;
		using (response = request.GetResponse() as HttpWebResponse)  
			{  
			reader = new StreamReader(response.GetResponseStream());  
			buffer=reader.ReadToEnd(); 
			reader.Close();
			}

		if((response==null) || (buffer.IndexOf("[PUTTY-FICS-OUT-BUFFER-OK]")<0))
			{
			// do nothing, next request can be successfull...(especially in the middle of a game)
			//	FicsAccessible=false; Let3DknowAccess();
			}
		}
		}
		
	// Update is called once per frame
	void Update ()
		{
		if((FicsConnTime>0) && ((Time.time-FicsConnTime)>4))
			{
			ConnectServer2(); FicsConnTime=0;			// wait 4 seconds to check sockets...& maybe Esc press...
			}
		if((Time.time-lastUpdate)>time_difference)		// a bit interval just not to make connection too tied... (actually it's local & fast enaugh)
			{
			ReadFromServer();				// Read input streamed data...
				
			SendLinesOUT("");				// Send to server lines what & when    needed...
				
			lastUpdate=Time.time;
				
			Status2MainScript();		// Informs main script about current status... - no direct memory access via 3D...
			}
			
		if(status==9) wantPlayTime=Time.time;					
		if((status==10) && ((Time.time-wantPlayTime)>20)) status=9;		// If long waiting then look for other game...
		}
	
	void Status2MainScript()
		{
		(GameObject.Find("Script2")).SendMessage("TcpStatus", status.ToString());
		}
		
	void Movement(string move)
		{
		MoveMade=move;
		}
		
	void StartNextGame(string parm)
		{
		status=9;		// Search for next game...
		}
	void AutoMode(string sw)
		{
		autoCmds =(sw=="ON");		// When automated processing mode on...
		}
		
	// On monobehaviour ends, this is called....
	void OnApplicationQuit()
		{
		SendToServer("EXIT_FICSCALL");		// FicsCall forced exit - just not to left running in memory...
		}
}