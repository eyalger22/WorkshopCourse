# Workshop-20B
Workshop on Software Engineering Project - group 20B
 
By:
* Eyal German
* Eldad Vasker
* Daniel Pikovsky
* Achia Amitay


How to use an Init file:
The init file supports the following operations:
 - register users
 - open shops
 - appointing managers and owners
 - adding items to shops
 - login to users
 - logout from users

How the init file should look:
The file should be called Init_File.txt and should be in the Init files directory of the project
Here is how you format each section:
Registering users:

    _Register:
    <username>,<password>,<email>,<address>,_<phone>,#<birthDate>

The phone and birthDate are optional so we identify them by special characters


Opening shop:

    _Shop:
    <username>,<shopName>,<shopAddress>,<bank>


Appointing managers:

    _Manager:
    <appointersUserName>,<shopName>,<appointedUserName>


Appointing owners:

    _Owner:
    <appointersUserName>,<shopName>,<appointedUserName>


Adding items to shops:

    _Product:
    <userName>,<shopName>,<productName>,<price>,<category>,<description>,<amount>

Login to users:

    _Login:
    <userName>,<password>

Logout from users:

    _Logout:
    <userName>


An example to how the Init file should look like:

    _Register:
    user12,user12,user12@gmail.com,address 12
    _Register:
    user13,user13,user13@gmail.com,address 13
    _Register:
    user14,user14,user14@gmail.com,address 14
    _Register:
    user15,user15,user15@gmail.com,address 15
    _Login:
    user12,user12
    _Shop:
    user12,shop1,address s1,bank1
    _Product:
    user12,shop1,Apple,20,Fruits,An apple,10
    _Logout:
    user12