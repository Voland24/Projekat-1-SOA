Inner API u Node (Moleculer) ima kontakt sa MongoDB


Pre nego sto ga stavis u docker, sa npm run dev mozes da pokrenes lokalno sve i na portu 8080 su rute 

Express se pokrene u Dockeru i rute koje ne rade s bazom mogu da se okinu

Sve rute koje koriste bazu timeoutuju kad probaju da dovuku nesto

Ovo se desava jer app proba da pristupi Moduelu pre nego sto je baza konektovana izgleda

Mada iako sam rekao da treba da se saceka da je baza povezana pre nego da se prave kontejneri i 
da se povezivanje s bazom u app.js radi u require, i dalje nece



Express API 
    docker build -t test-node-app .
    docker run -p 49165:8080 -d test-node-app  // moze i 8080:8080 ako hoces na isti port da budu i slika i api

    Na portu 49165 moze da se dohvati

Express API i Mongo moze sa "docker compose up" da se pokrenu
BITNO: pre toga u db_connect.js promeni url u "mongodb://mongo:27017/bookstore"
Oba se konektuju ali isto, sve rute koje koriste modele timeoutuju jer se desava isti problem

Probao sam sve i svasta s neta ali nista ne radi. Ako imas ideju kako da se ovo uradi, cepaj slobodno samo mi kazi da pullam

    http://localhost:8080/ su sve rute 

    GET: 
        /getBookByTitle
        /getBookByAuthor
        /getBookByISBN

    POST:
        /insertNewBook

    PUT:
        /updateBookQuantity
        
        //salji mi ili ISBN ili naslov, preko oba cu da je trazim
        /*
        {
                "bookTitle" : "",
                "ISBN" : "",
                "operation" : -1 ako uzima, +1 ako vraca knjigu, kao broj ga salji
        }
        Ostavi jedno prazno tj. ono po kojem se ne radi pretraga
        */ 

    DELETE:
        /deleteABook

        /*
            {
                 "bookTitle" : "",
                 "ISBN" : "",
            }

            ovaj oblik
            */ 

