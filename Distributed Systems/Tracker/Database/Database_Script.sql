
CREATE TABLE Torrent (
 hash varchar(100) NOT NULL,
 id varchar(40)  NOT NULL,
 ip_address varchar(40)  NOT NULL,
 port int(11)  NOT NULL,
 amount_left int(11) NOT NULL,
 time_entered timestamp NOT NULL DEFAULT current_timestamp(),
 PRIMARY KEY (hash, id, ip_address) 
);
