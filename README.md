There is a **MeterReadingController** which is exposing a Http Post Endpoint named **meter-reading-uploads** and all it has is a **MeterReadingService** which just gets passed the csv file from the controller and apply validation rules on the data on file. The service also uses **ConsumerRepository** for database queries & does some mapping behind the scenes.

The **ConsumerRepository** is connecting with database using the DBContext from EF and running these queries and/or saving to the database.

Additionally I have **Srilogger** to write any rule violations or  critical errors to a text file in a root Log folder. This is very convenient to trouble shoot issues.

To test the whole thing I have added **Swagger** API tester to quickly upload a file and test it.
