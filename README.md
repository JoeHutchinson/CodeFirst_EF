# CodeFirst_EF

This project is an example of allowing a user to enter a URL into a webpage, the service will then visit the URL, parse all visible text, count the words and persist those words to a database. Once persisted the UI displays a tag cloud to the user visualising the top 100 counted words, an admin page shows the same information visualised as a table.

## Approach
I've been interested in EF Always Encrypted feature for a while now and used this project as an opportunity to try it out in a realistic scenario. This design decision meant I couldn't use .NET Core 2 for the backend as EF Core doesn't currently support Always Encrypted connections (no idea why, it's been requested for over a year and it's "in their backlog"). Anyway Always Encryption is great because it is assymetric encryption where the service code knows nothing about encryption, its all handled in ADO.NET layer with keys stored in Windows Store or Azure Vault.

### Storage and Hashing Design
I wanted to abstract away the concept of using SQL as a store, with the benefit being ease of adoption of different technologies such as Redis, Oracle DB or possibly Kafka streams. IRepository interface is this abstract, EntityFrameworkRepository is one implementation handling SQL interaction. Speaking of EntityFrameworkRepository have a look at the Get method, I really like how flexible it is, allowing the caller to define what they want without losing encapsulation.

Another side effect of using Always Encrypted means I have to use MigrateDatabaseToLatestVersion database initializer (it's the only one that supported), it's the first time I've used this particular initializer its quite nice as it generates the initial creation script for you then on your free to do as you wish.

I wanted to define behavior of how an entity is used through the backend using attributes specified on the properties of the entity, the idea being that should be the single point you change if say you want to start hashing a new field. For this I've created two attributes:
* HashAttribute placing it on a field will inform storage repository to hash this field
* HashKeyAttribute can only be placed on one field, it indicates the key to store the salt against in the SaltCache

Similiar to swapping out SQL store typically hashing algorithms have short lifespans necessitating in change. For this reason I created IHashProvider interface allowing separation between the business logic of hashing (HashRepository) and the complexities of actually hashing PBKDF2Provider. 

Words Repository is connecting layer between frontend and backend, a repository that abstracts away dealing with EF, I was planning to make this more like a factory, you tell it what Entity you want and it gives you a repository capable of handling it however just ran out of time so it directly uses EFRepository. I particularly like how Top 100 words is just an input to this repo and is passed through to the underlying layer.

I use quite a lot of DI in the solution, given time I think I'd refactor this down a bit as it can easily get into dependency hell, for IoC I'm using Unity. Unity reports a few exceptions when running, as far as I can tell its not effecting system behavior I've just not had time to look into them.

### HTML Parsing
I was running pretty short on time here but I've made a simple collector/crawler using HTMLAgility nuget package to get the HTMLDoc. Particular points I don't like is how difficult it is to test Collect, I'd likely use DI here to have IParseFactory injected into this method allowing for more mocking and separation of responsibility of fetching to reading and possibly even counting. Time was really against me by this point having focussed on backend first.

Even though the architecture of parsing is meh, performance wise it's quite fast. I use a Dictionary in the counting logic purely to speed up counting allowing close to O(1) time complexity.

### Front end
By this point I was really pushed for time. I was initially planning to run a React UI with components for Nav, Tag Cloud and Word Table along with OAuth provided by Auth0. By the time I got round to building it I had about 30 minutes (and the wife was getting scary). So instead I've opted for a bare bones approach to get the basic functionality so we don't have to use Postman requests.

It's simply a SPA with JQuery behind, it has a weird behavior when you press Enter on the URL box, not had chance to fix that. The tag cloud is displayed after POST is returned. I realised while writing this I didn't catch errors from the POST sorry about that.

The controllers are particularly neat, I added notes to the Crawl controller so I won't repeat those here.

### Testing
Benefit of using DI is that all the test are very straight forward. I have unit and integration test, where I consider intregration test a test which requires a DB.

All integration tests extend a common test setup class IntegrationTestSetup, coupled with a TestDB context it allows for a testing DB that uses the same schema migrations as the production DB.

I've added tests around most functionality, there's still more to test but time was against me but hopefully you can see the approach my testing style from what I have done.

### Configuration
You can control whether to use Always Encrypted and hashing features through app.config settings. By default I've switched hashing off as it slows the code down considerably.

#### Performance
From the outset I wanted to be sympathetic towards SQL, bulk updates in EF by default are highly inefficient so instead I opted to use a two table model. The first table acts like a temporary table allowing all just read words to be bulk uploaded using SqlBulkCopy, I've not had chance to add a TVP (table value property) to this but even without it the performance of this is good. Once updated a stored procedure does a SQL Merge from temporary table to main table, matching on Id field, matches have their counts incremented by the value in temporary table and no match has the record from temporary table inserted. This approach is nice as its the fastest approach to get this kind of information into SQL that I know of. This all means that the main table contains all read words, that goes beyond the top 100 displayed to the user, nice as it means that words currently not in the top 100 could get into the top 100 based on subsequent crawls of URLs.

Hashing can be very slow, I adopted a simple hashing algorithm that I used in a previous project however it's slow using it to hash hundreds if not thousands of words. I realised this late into the build and didn't have time to swap it out, instead I optimised the code around it. These optimisations are keeping an in-memory salt cache, keyed of the unhashed word, on service start it reads the main table and initialises this allows Upsert logic to do only one DB write to temporary table, no reads required. The other optimisation was to use parallel foreach to hash entities, it's speeded up execution a lot but I still think it sucks. I've disabled hashing by default in the app.config you can change HashingEnabled to True and you'll see this massive slow down.

### Oddities
Project name CodeFirst_EF I'm using this as basis for a home project so doesn't make much sense in this context.
CodeFirst_EF is console project, I left it like this as it can be useful to run and see the basic flow without the UI, more to make mine and you life easier, wouldn't do this for a real project.

### How to run
1. Need to create CountVonCountDB, I found EF sometimes has issues here so I've included a DB script to create the database and my mdf and log files. See GettingStarted folder, just drop the files into a path you can access update the script location and run, you should end with an empty DB called CountVonCountDB. This step may need to be repeated for the TestDB.
2. Create a assymetric key on your SQL Server code by default expects column store key of CEK_Auto1. This can be done in a number of ways. The easiest I've found is just going to CountVonCountDB, creating a dummy table with a column, right clicking the table in Object Explorer and click Encrypt Columns, ensure you create a Deterministic type key but everything else can be default. Other way is using the powershell script CreateKeys.ps1 and supplying parameters for your environment.
