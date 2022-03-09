#Internationalization(i18n) of SHJP



i18n is a separate web service developed to maintain and manage activities related to internationalization (i18n) of SHJP project. The i18n service is environment agnostic and the one single stop where language translations are kept for all platforms including Portal, Egate, REST APIs, Mobile apps and upcoming services such as HRMS, Service Desk.



**How it works**



The i18n service can be accessed through a web portal(say i18n.shjpolice.gov.ae) where two types of users are only authenticated; **developer** and **translator**. Authentication services are developed using firebase since SHJP already has firebase account and authentication services are free in firebase. However all the core data are kept in one of the mongoDB instances(replication set) in SHJP premise



No self signup option is provided. Users can be added manually by PITS management using the firebase UI. Email address and password has to be set for each user who have to access the i18n service. Email addresses having domain as [pitsolutions.com](https://www.pitsolutions.ch/en/) will be automatically treated as **developer** and those with domain [shjpolice.gov.ae](https://shjpolice.gov.ae/) as **translator**.



>**Note:** Registering email addresses with any other domain doesn't have any impact on the system since the system wouldn't allow them to interact with the data



**Adding a new phrase**



Only users with developer role can add a new phrase. While adding a phrase the developer has to provide the below parameters:

1. Key*
1. English part of the phrase*
1. Arabic part of the phrase*
1. List of platforms this key can be used*(egate, portal, api, mobile)

>**Key Redundancy:** Developer never can add a new phrase if the key of that phrase is already used in any of the selected platforms. For example: if a key named as x_key is already added for egate platform, it cannot be added again for egate platform. However it can be added for any other platform 


>_This check is done while updating a phrase also_





**Similarity check**



While adding a new phrase, the i18n service will check it's english part against the phrases in the database for similarity. A maximum of 10 similar phrases can be returned by the i18n service and the same can be scrutinized by the developer before saving the new phrase.



_For instance:_ If the developer is trying to add a new phrase with english part as "hello world" and an already added phrase with "hi world"  is present in the database, the system will smell similarity and prompt the user about the similarity and recommend to use the same instead of adding a new one.



**Updating a phrase(translation)**



A phrase can be updated with a new english part, arabic part or the platforms in which it is used. An user with developer role can always update any part of the phrase. However an user with translator role can only update the arabic part of the phrase.



A phrase can be considered as **translated** only when it's arabic part is changed and the change is done by an user with translator role. If a developer changes the arabic part in any case, that phrase will be again marked as **non-translated**. This behavior will be useful to the system to prominently notify the translators which all phrases requires their attention.



**Deleting a phrase**



Since phrases are directly depended to application code, mistakenly deleting a phrase can cause troubles. So there are extra conditions and verification processes implemented before deleting a phrase.



1. Only user with **developer** role can delete a phrase
1. Any phrase which are older than **30 days** in the system cannot be deleted
1. Any phrase which are marked as **translated** cannot be deleted
1. The user should retype the exact key value of the phrase in a prompt window before deleting a phrase(to prevent accident clicks)


>**History:** Detailed history of each phrase are kept in the database and the same can be used to investigate alterations happened to them



**How the platforms interact with i18n service?**



The i18n service provides an API to download the phrases required for a specific platform as a json file. All the platforms including Egate, Portal, Rest APIs and Mobile Apps can download this file on their application startup logic and do the necessary amendments and further use them as they are used currently.

Also note that this end point is so sophisticated that it caches the json file of all platforms separately in it's in-memory until has some change detected in any of the phrases of each platform. Also it has the ability to compute hash of it's content so that the callers of this end point will be notified with a 304 status code, meaning no change since last downloaded, thus allowing the caller to use their cached version of the file(if any) instead downloading it again.