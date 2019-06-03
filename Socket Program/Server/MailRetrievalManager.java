/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package Server;

import static Server.Server.allMail;
import java.io.*;
import java.io.FileReader;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Random;
import sun.misc.BASE64Decoder;
import sun.misc.BASE64Encoder;


public class MailRetrievalManager {

    ArrayList<MailManager> mailBox = new ArrayList<MailManager>();
    ArrayList<MailManager> spamMailBox = new ArrayList<MailManager>();
    ArrayList<MailManager> deletedMailBox = new ArrayList<MailManager>();
    DataOutputStream output = null;

    String address = null;
    String user = "";
    String password = null;
  
    int mailAmount = 0;
    String userHash = null;
    String passwordHash = null;
    String accountLockedString = null;
    boolean accountLocked = false;

    public void SetUser(String inUser) {
        user = inUser;
    }

    public void SetPassword(String inPassword) {
        password = inPassword;
    }


    public void AddToMailBox(MailManager mail, String mailType) {
        //adds incoming mail to the correct lists using the mailtype
        if (address.equals(mail.GetTo())) {
            if (mailType.toUpperCase().equals("INBOX")) {
                if (mailBox == null) {
                    mailBox = new ArrayList<MailManager>();
                }

                //if the mail is flagged, it is put at the top of the list
                if (mail.GetFlag().equals("yes")) {
                    mailBox.add(0, mail);
                } else {
                    mailBox.add(mail);
                }
            }

            if (mailType.toUpperCase().equals("SPAM")) {
                if (spamMailBox == null) {
                    spamMailBox = new ArrayList<MailManager>();
                }
                spamMailBox.add(mail);
            }

            if (mailType.toUpperCase().equals("DELETED")) {
                if (deletedMailBox == null) {
                    deletedMailBox = new ArrayList<MailManager>();
                }

                deletedMailBox.add(mail);
            }
        }
        boolean alreadyExists = false;

        //a for loop to make sure no duplicates are added
        for (MailManager mails : allMail) {
            if (mails.equals(mail)) {
                alreadyExists = true;
            }
        }

        //add to the main list if no duplicate is found
        if (!alreadyExists && !mailType.toUpperCase().equals("DELETED")) {
            mailAmount++;
            allMail.add(mail);
        }
    }

    public void PopulateMailBox() throws IOException {

        /*goes through each mail found in the MailStoragefile
          and creates a MailManager instance for each mail
        */
        allMail.clear();
        mailBox.clear();
        spamMailBox.clear();
        deletedMailBox.clear();

        File file = new File("MailStorage.txt");

        if (file.exists()) {
            FileReader fileReader = new FileReader("MailStorage.txt");
            BufferedReader bufferedReader = new BufferedReader(fileReader);

            String firstLine = bufferedReader.readLine();
            
            if (firstLine != null) {

                //each mail is seperated by a ] character
                String[] mail = firstLine.split("]");

                for (int i = 0; i < mail.length; i++) {
                    //each segment of the mail is seperated by a comma
                    String[] mailParts = mail[i].split(",");
                    
                    MailManager newMail = new MailManager();

                    newMail.SetType(mailParts[0]);
                    newMail.SetFlag(mailParts[1]);
                    newMail.SetFrom(mailParts[2]);
                    newMail.SetTo(mailParts[3]);
                    newMail.SetDate(mailParts[4]);
                    newMail.SetSubject(mailParts[5]);

                     //variables used for the data part of the mail
                    int length = 5;

                    while (length < mailParts.length) {
                        newMail.AddData(mailParts[length]);
                        length++;
                    }

                    AddToMailBox(newMail, newMail.type);
                }
            }
        }
    }

    public void FlagMail(String mailNumber) throws IOException {
        
        //toggles flagging on and off on selected inbox mail
        int number = Integer.parseInt(mailNumber) - 1;

        //ensures the mail number exists
        if (number < mailBox.size() && number > -1) {
            String flag = mailBox.get(number).GetFlag();

            //changes the mails flag option and updates lists accordingly
            if (flag.equals("no")) {
                mailBox.get(number).SetFlag("yes");
                mailBox.add(0, mailBox.get(number));
                allMail.remove(mailBox.remove(number + 1));
                allMail.add(mailBox.get(0));
            } else {
                mailBox.get(number).SetFlag("no");
                mailBox.add(mailBox.get(number));
                allMail.remove(mailBox.remove(number));
                allMail.add(mailBox.get(mailBox.size() - 1));
            }

            output.writeUTF("+OK mail " + (number + 1) + " flagged/unflagged");
        } else {
            output.writeUTF("-ERR mail does not exist");
        }
    }

  
    public void ShowInbox() throws IOException {
        //shows brief information on all mail found in the mailbox list
        if (mailBox != null) {
            int number = 1;
            output.writeUTF("+OK " + mailBox.size() + " mails found");
            for (MailManager mail : mailBox) {
                output.writeUTF("----------Mail " + number + " ----------");
                output.writeUTF("Flagged : " + mail.GetFlag());
                output.writeUTF("From : " + mail.GetFrom());
                output.writeUTF("Date: " + mail.GetDate());
                output.writeUTF("Subject : " + mail.GetSubject());
                number++;
            }
        } else {
            output.writeUTF("No mail present");
        }

    }

   
    public void ShowMailInbox(String mailNumber) throws IOException {
        //shows a all data from a selected mail from the mailbox list
        int number = Integer.parseInt(mailNumber) - 1;

        if (mailBox != null) {
            //ensures the selected number exists on the list
            if (number < mailBox.size() && number > -1) {
                output.writeUTF("+OK mail " + number + " open");
                output.writeUTF("Flagged : " + mailBox.get(number).GetFlag());
                output.writeUTF("From : " + mailBox.get(number).GetFrom());
                output.writeUTF("To : " + mailBox.get(number).GetTo());
                output.writeUTF("Date: " + mailBox.get(number).GetDate());
                output.writeUTF("Subject : " + mailBox.get(number).GetSubject());
                for (String str : mailBox.get(number).data) {
                    output.writeUTF(str);
                }
            } else {
                output.writeUTF("-ERR mail " + number + 1 + " does not exist");
            }
        } else {
            output.writeUTF("No mail present");
        }
    }

    public void ShowSpamMailBox() throws IOException {
        //shows brief information on all mail in the spambox list
        if (spamMailBox != null) {
            int number = 1;
            output.writeUTF("+OK " + spamMailBox.size() + " mails found");
            for (MailManager mail : spamMailBox) {
                output.writeUTF("----------Mail " + number + " ----------");
                output.writeUTF("From : " + mail.GetFrom());
                output.writeUTF("Date: " + mail.GetDate());
                output.writeUTF("Subject : " + mail.GetSubject());
                number++;
            }
        } else {
            output.writeUTF("No mail present");
        }
    }

  
    public void ShowMailSpam(String mailNumber) throws IOException {
        //shows all information on a selected mail from the spambox list
        int number = Integer.parseInt(mailNumber) - 1;

        if (spamMailBox != null) {
            //ensures the selected number exists
            if (number < spamMailBox.size() && number > -1) {
                output.writeUTF("+OK spam mail " + number + " opened");
                output.writeUTF("From : " + spamMailBox.get(number).GetFrom());
                output.writeUTF("To : " + spamMailBox.get(number).GetTo());
                output.writeUTF("Date: " + spamMailBox.get(number).GetDate());
                output.writeUTF("Subject : " + spamMailBox.get(number).GetSubject());
                for (String str : spamMailBox.get(number).data) {
                    output.writeUTF(str);
                }
            } else {
                output.writeUTF("-ERR mail " + (number + 1) + " does not exist");
            }
        } else {
            output.writeUTF("No mail present");
        }

    }

    public void ShowDeletedMailBox() throws IOException {
        //shows brief information on all deleted mail from the deleted list
        if (deletedMailBox != null) {
            int number = 1;
            output.writeUTF("+OK " + deletedMailBox.size() + " mails found");
            for (MailManager mail : deletedMailBox) {
                output.writeUTF("----------Mail " + number + " ----------");
                output.writeUTF("From : " + mail.GetFrom());
                output.writeUTF("Date: " + mail.GetDate());
                output.writeUTF("Subject : " + mail.GetSubject());
                number++;
            }
        } else {
            output.writeUTF("No mail present");
        }
    }

    public void ShowMailDeleted(String mailNumber) throws IOException {
        //shows all information on a selected mail from the deleted list
        int number = Integer.parseInt(mailNumber) - 1;

        if (deletedMailBox != null) {
            //ensures the number is on the list
            if (number < deletedMailBox.size() && number > -1) {
                output.writeUTF("+OK deleted mail " + number + " opened");
                output.writeUTF("From : " + deletedMailBox.get(number).GetFrom());
                output.writeUTF("To : " + deletedMailBox.get(number).GetTo());
                output.writeUTF("Date: " + deletedMailBox.get(number).GetDate());
                output.writeUTF("Subject : " + deletedMailBox.get(number).GetSubject());
                for (String str : deletedMailBox.get(number).data) {
                    output.writeUTF(str);
                }
            } else {
                output.writeUTF("-ERR mail " + (number + 1) + " does not exist");
            }
        } else {
            output.writeUTF("No mail present");
        }
    }

    
    public void SendToInbox(String mailNumber, String type) throws IOException {
        //sends spam or deleted mail to the mailbox list
        int number = Integer.parseInt(mailNumber) - 1;

        //this removes from the spam list and adds to the mailbox
        if (type.contains("spam")) {
            if (number < spamMailBox.size() && number > -1) {
                spamMailBox.get(number).SetType("inbox");
                allMail.remove(spamMailBox.get(number));
                AddToMailBox(spamMailBox.remove(number), "inbox");
                output.writeUTF("+OK mail " + (number + 1) + " moved to inbox");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }
        
        //removes from the deleted list and adds to the mailbox
        if (type.contains("deleted")) {
            if (number < deletedMailBox.size() && number > -1) {
                deletedMailBox.get(number).SetType("inbox");
                AddToMailBox(deletedMailBox.remove(number), "inbox");
                output.writeUTF("+OK mail " + (number + 1) + " moved to inbox");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }
    }

    public void SendToSpam(String mailNumber, String type) throws IOException {
        //sends mail to the spam list
        int number = Integer.parseInt(mailNumber) - 1;

        //mailbox mail is sent to the spam list
        if (type.contains("inbox")) {
            if (number < mailBox.size() && number > -1) {
                mailBox.get(number).SetType("spam");
                allMail.remove(mailBox.get(number));
                AddToMailBox(mailBox.remove(number), "spam");
                output.writeUTF("+OK mail " + (number + 1) + " moved to spam");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }

        //deleted mail is sent to the spam list
        if (type.contains("deleted")) {
            if (number < deletedMailBox.size() && number > -1) {
                deletedMailBox.get(number).SetType("spam");
                AddToMailBox(deletedMailBox.remove(number), "spam");
                output.writeUTF("+OK mail " + (number + 1) + " moved to spam");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }
    }

  
    public void SendToDelete(String mailNumber, String type) throws IOException {
        //sends mail to the deleted list
        int number = Integer.parseInt(mailNumber) - 1;
        
        //sends mail from the mailbox to the deleted list
        if (type.contains("inbox")) {
            if (number < mailBox.size() && number > -1) {
                mailBox.get(number).SetType("deleted");
                allMail.remove(mailBox.get(number));
                AddToMailBox(mailBox.remove(number), "deleted");
                output.writeUTF("+OK mail " + (number + 1) + " moved to deleted");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }

        //sends mail from the spambox to the deleted list
        if (type.contains("spam")) {
            if (number < spamMailBox.size() && number > -1) {
                spamMailBox.get(number).SetType("deleted");
                allMail.remove(spamMailBox.get(number));
                AddToMailBox(spamMailBox.remove(number), "deleted");
                output.writeUTF("+OK mail " + (number + 1) + " moved to deleted");
            } else {
                output.writeUTF("-ERR mail does not exist");
            }
        }

    }

    public boolean UserCheck() throws IOException {
        //checks the username entered matches with encrypted file username
        if (user.equals(userHash)) {
            return true;
        }
        return false;
    }


    public boolean PasswordCheck(ArrayList<ClientManager> clientManagerList) throws IOException {
       //checks the password entered matches with encrypted file password
        if (password.equals(passwordHash) && !accountLocked) {
            AccountLock(true);
            return true;
        }
        
        //if the account is locked a check is made to see if it's currently in use
        if(accountLocked)
        {
            //account could be locked from exiting the program without quit command
            //this check ensures access is only granted if the details are not already
            //in use
            for(ClientManager clients : clientManagerList)
            {
                if(clients.name != null && clients.name.equals(user))
                {
                    
                    output.writeUTF("-ERR account in use");
                    return false;
                    
                }
            } 
            //if no match is found the loggin is successful
            return true;
        }
        
        output.writeUTF("-ERR incorrect password");
        return false;
    }

   public void AccountLock(boolean lock) throws IOException
   {
       File file = new File("UserData.txt");
        FileWriter fileWriter = new FileWriter(file);
        PrintWriter writer = new PrintWriter(fileWriter);
        
        writer.print(encrypt(userHash) + ",");
        writer.print(encrypt(passwordHash) + ",");
        writer.print(address + ",");
        
        if(lock)
        {
            writer.print("yes");
        }
        
        else
        {
            writer.print("no");
        }
        
        writer.close();
        
   }
    public void GetDetails() throws IOException {
        //fetches details for the user log in and decrpyts ready for comparison
        FileReader fileReader = new FileReader("UserData.txt");
        BufferedReader bufferedReader = new BufferedReader(fileReader);
        
        String[] part = bufferedReader.readLine().split(",");

        userHash = part[0];
        passwordHash = part[1];
        address = part[2];
        accountLockedString = part[3];
        
        if(accountLockedString.contains("yes"))
        {
            accountLocked = true;
        }

        //these variables are now decrypted
        userHash = decrypt(userHash);
        passwordHash = decrypt(passwordHash);
    }

   
    public static String encrypt(String str) {
        //encrpyts a string 
        BASE64Encoder encoder = new BASE64Encoder();
        
        //create random bytes and encode them and the string into a new array
        byte[] salt = new byte[8];
        Random rand = new Random();
        rand.nextBytes(salt);
        return encoder.encode(salt) + encoder.encode(str.getBytes());
    }

  
    public static String decrypt(String encryptedString) {
        //decrypts a string 
        if (encryptedString.length() > 12) {
            //decode the the string to a byte array
            String cipher = encryptedString.substring(12);
            BASE64Decoder decoder = new BASE64Decoder();
            try {
                //return as a string
                return new String(decoder.decodeBuffer(cipher));
            } catch (IOException e) {
            }
        }
        return null;
    }
}
