package Client;

import java.io.*;
import java.net.*;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Client {

    //Main Method:- called when running the class file.
    public static void main(String[] args) {

        int portNumber = 0;

        //checks to see if port number is valid and within range, defaults to 15882 if not
        try {
            portNumber = Integer.parseInt(args[0]);

        } catch (Exception ex) {
            portNumber = 15882;
        }

        if (portNumber < 2048) {
            portNumber = 15882;
        }

        String serverIP = "localhost";

        try {
            //Create a new socket 
            Socket soc = new Socket(serverIP, portNumber);

            // create new instance of the client reader thread, intialise it and start it running
            ClientReader clientRead = new ClientReader(soc);
            Thread clientReadThread = new Thread(clientRead);
            clientReadThread.start();

            // create new instance of the client writer thread, intialise it and start it running
            ClientWriter clientWrite = new ClientWriter(soc);
            Thread clientWriteThread = new Thread(clientWrite);
            clientWriteThread.start();

        } catch (Exception except) {
            System.out.println("Error --> " + except.getMessage());
        }
    }
}

//This thread is responcible for writing messages
class ClientWriter implements Runnable {

    Socket cwSocket = null;

    public ClientWriter(Socket outputSoc) {
        cwSocket = outputSoc;
    }

    public void run() {
        try {
            //set up buffreader and output stream so messages can be sent
            DataOutputStream dataOut = new DataOutputStream(cwSocket.getOutputStream());
            BufferedReader stdln = new BufferedReader(new InputStreamReader(System.in));
            String userInput;
           
            

            while (true) {
                userInput = stdln.readLine();
                dataOut.writeUTF(userInput);
                dataOut.flush();
            }
        } catch (Exception except) {
            //Exception thrown write message to console
            System.out.println("Error in Writer--> " + except.getMessage());
        }
    }
}

//thread for reading messages
class ClientReader implements Runnable {

    Socket cwSocket = null;

    public ClientReader(Socket outputSoc) {
        cwSocket = outputSoc;
    }

    public void run() {
        try {
            //Create the outputstream to send data through
            DataInputStream dataInput = new DataInputStream(cwSocket.getInputStream());
            // System.out.println("Client reader running");

            //reads messages from the server
            while (true) {
                //if the exception is thrown it means the client has quit
                try {
                    String incomingLine = dataInput.readUTF();
                    System.out.println(">" + incomingLine);
                } catch (IOException ex) {
                    dataInput.close();
                    System.exit(0);
                }
            }
            //close the stream once we are done with it

        } catch (Exception except) {
            //Exception thrown (except) when something went wrong, pushing message to the console
            System.out.println("Error in Reader--> " + except.getMessage());
        }
    }
}
