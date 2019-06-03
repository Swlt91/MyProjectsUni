package Server;

import static Server.Server.allMail;
import java.io.PrintWriter;
import java.net.Socket;
import java.util.ArrayList;
import java.io.*;
import java.security.*;
import java.util.logging.Level;
import java.util.logging.Logger;
import sun.misc.BASE64Decoder;
import sun.misc.BASE64Encoder;
import java.util.Random;

public class ClientManager {

    public Socket socket = null;
    public DataInputStream input = null;
    public DataOutputStream output = null;

    String name = null;
    String address = null;;


    public ClientManager(Socket insoc) throws IOException {
        socket = insoc;
        input = new DataInputStream(insoc.getInputStream());
        output = new DataOutputStream(insoc.getOutputStream());
    }

    public synchronized String GetName() {
        return name;
    }

    public void SetName(String inName) {
        name = inName;
    }

    public synchronized String GetAddress() {
        return address;
    }

    public void SetAddress(String inAddress) {
        address = inAddress;
    }

    public void ClientQuit() throws IOException {
        try {
            input.close();
            output.close();
            socket.close();
        } catch (IOException e) {
            System.out.println("Error when quitting -->" + e);
        }
    }

    public boolean ValidEmail(String inEmail)
    {
         if(inEmail.contains("@"))
         {
             String[] splitEmail = inEmail.split("@");
             
             if(splitEmail.length > 1)
             {
                 if(splitEmail[1].contains(".com") || splitEmail[1].contains(".co.uk"))
                 {
                     return true;
                 }
             }
         }
         
         return false;
    }

    public void MailStorage() throws IOException {
        //write a file to the projects destination, containing all mails
        File mailStorage = new File("MailStorage.txt");

        /*  if (!mailStorage.exists()) {
            mailStorage.createNewFile();
        }*/
        FileWriter fileWriter = new FileWriter(mailStorage);
        PrintWriter writer = new PrintWriter(fileWriter);

        for (MailManager mail : allMail) {
            writer.print(mail.GetType() + ",");
            writer.print(mail.GetFlag() + ",");
            writer.print(mail.GetFrom() + ",");
            writer.print(mail.GetTo() + ",");
            writer.print(mail.GetDate() + ",");
            writer.print(mail.GetSubject() + ",");

            for (String line : mail.GetData()) {
                writer.print(line + ",");
            }

            writer.print("]");

        }
        writer.close();
    }
    
    public boolean StringToInt(String number) throws IOException
    {
        try
        {
            int convert = Integer.parseInt(number);
            return true;
        }
        catch(Exception e)
        {
            output.writeUTF("-ERR invalid input");
            return false;
        }
    }
}
