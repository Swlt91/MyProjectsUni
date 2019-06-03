
package Server;

import static Server.Server.allMail;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;

public class MailManager {
    String type = null;
    String flagged = null;
    String from = null;
    String to = null;
    ArrayList<String> rcptToList = null;
    String date = null;
    String subject = null;
    ArrayList<String> data = null;

    public String GetType() {
        return type;
    }

    public void SetType(String inType) {
        type = inType;
    }
    
    public String GetFlag() {
        return flagged;
    }

    public void SetFlag(String inFlag) {
        flagged = inFlag;
    }
    
    public synchronized String GetFrom() {
        return from;
    }

    public void SetFrom(String inFrom) {
        from = inFrom;
    }

    public synchronized String GetTo() {
        return to;
    }

    public void SetTo(String inTo) {
        to = inTo;
    }

    public synchronized String GetSubject() {
        return subject;
    }

    public void SetSubject(String inSubject) {
        if(inSubject == null)
        {
            inSubject = "";
        }
        subject = inSubject;
    }
    
    public void SetDate(String inDate) {
        
        if(inDate == null)
        {
        //sets to the date the mail was sent and formats 
        DateFormat dateFormat = new SimpleDateFormat("dd MMM yy HH:mm:ss");
        Date dateNow = new Date();
        date = dateFormat.format(dateNow).toString();
        }
        
        else
        {
            date = inDate;
        }
    }

    public synchronized String GetDate() {
        return date;
    }

    public void AddRcpt(String client) {
        //adds recipients to list for the mail
        if (rcptToList == null) {
            rcptToList = new ArrayList<String>();
        }
        rcptToList.add(client);
    }

     public synchronized ArrayList<String> GetRcptToList() {
         if(rcptToList == null)
         {
             rcptToList = new ArrayList<String>();
         }
        return rcptToList;
    }
     
    public ArrayList<String> GetData() {
        return data;
    }

    public void AddData(String incomingData) {
        //adds lines to the data array for the body of the mail
       if(data == null)
       {
           data = new ArrayList<String>();
       }      
       data.add(incomingData);
    }

   
    public void SendMessage() {
       
        for (String clients : rcptToList) {
            //send to all rcpt to
            SetType("inbox");
            SetFlag("no");
            SetTo(clients);
            SetDate(null);
            allMail.add(this);
        }
       
    }
    
}
