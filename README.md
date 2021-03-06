The intention of this program is to get a large set of randomized data using a url and save it to a database (SQL is preferred but there is an option for a non-persistant database).
This data can then be used to report of the validity of the data (i.e. whether there are enough data points) and then try and identify if there is any data that does not seem random.

Curently this has only been used and tested on https://www.reddit.com/r/random, and there are some things that are still specific to this url, I have tried to specify where this needs to be changed to allow for any url in the TODO List below.

Currently using MySQL database that is setup before with a table called 'subredditstats' with a 'name' VARCHAR(50) column and a 'time' DATETIME column already setup.

To read inputs from file, use following formatting
<Keyword>:<Value>
Where the keywords are:
- 'Host'		// Database host
- 'Port'		// Database port
- 'Database'	// Database name
- 'Username'	// Database username
- 'Password'	// Database password
- 'Table'		// Databases table name

TODO List:
- Change database setup so that the SQL table can have user specified columns names
    - OR setup a table in program to avoid issue (and assume tables already come under this name)
- Involve other SQL database types, not just MySQL
- Further checking of database validity (e.g. check to see what item has the least points, so if there is an item with 1, then it is unlikely this is enough points.
- Improve chart further, add options
- Solidify MVC by refactoring
    - Got lazy, move some aspects in Runner.cs to a separate model based class, and refactor existing to ensure view (CommandLineInterface.cs) is consistent and is separated from rest of code.
- Extend FormatResponse() in DefaultRunInfo.cs to allow user to specify what formatting is needed for each url (not just reddit)
