# Guidelines : database deployments(SHJP.API)
This document is intended to devise a new standard operating procedure to be carried out while  deploying a database along with a specific version of _SHJP.API_ application. This document only discuss the steps API developers should follow so that a new database version of specific environments are selectively upgraded. However this document does not deals with the internals of database objects involved in the upgradation. Also at the end of this document, we will learn to relinquish the dependency of releases over the [Migration Tracker sheet](https://docs.google.com/spreadsheets/d/1ENo3SZkzMShnr1nXAbHmaHeNSxbVhg9x-zYD-UcFWT0/edit#gid=0), the [old way](http://confluence.mypits.org:4090/display/KB/SQL+database+schema+migration+process) of database version management.
[[_TOC_]]
## Utility(EGATEAPI.DataAccess.Deploy)
A console utility has been placed under the data access layer of _SHJP.API_ repository itself, which would be the sole responsible entity to carry out database deployment activities. Each and every script files living inside this utility will be executed against selected environments in ordered manner. The main advantage of this utility is that it will keep the details of those scripts which are executed in the environment itself. In other words, the environment itself can let the utility know which all scripts are executed in it. Hence the previously used migration tracker, from where the database version of each environment was identified , will be deprecated as soon as we commence this SOP. 

> **No more Migration Tracker :** One of the motives to incorporate this utility was to completely avoid the tedious and error prone migration tracker from the deployment activities. 

## How to keep the scripts?

The usual script keeping process we used to follow earlier has to be continued as well with some slight changes. Only the way those scripts are kept and executed have been changed. 

In the new system, strict folder & file name policy has to be ensured. You cannot randomly name your folders & script files like you could do in the old way. 

#### Deploy scripts (no separation for data & schema)

1. Create a folder under the directory `EGATEAPI.DataAccess.Deploy\Scripts`. The name of the folder should represent the type of working branch you are currently working on. If the working branch is for a sprint, the name should be in the format: _S(#sprint_name#)_, where `S` identifies as a sprint. [All allowed formats are here](#allowed-namespaces)
2. Generate a valid file name from the [global counter](#SHJP.Utilities.Counter-aka-Global-Counter). Create script file using the generated value as the file name, under the folder you just made.
3. Go to the properties page`(right click --> Properties)` of the newly created script file and set the **Build Action** to **Embedded Resource**

#### Rollback scripts
1. Create a sub folder named **Rollbacks** under the folder you used for keeping deploy scripts
2. Create rollback script for each deploy script in the main folder. As for the file name of the rollbacks you create, use the number portion of the deploy script. For example, if your deploy script is `1025-e00da03b685a0dd18fb6a08af0923de0.sql` your rollback for that script should be `1025.sql`
3. Go to the properties page(right click --> Properties) of the newly created rollback script and ensure that the **Copy to Output Directory** is set to **Do not copy**, which should be the default

#### Ideal structure of scripts
- _EGATEAPI.DataAccess.Deploy\Scripts_
  - _S(wt1-001)_
    - _Rollbacks_
      - `175.sql`
      - `176.sql`
    - `175-82161242827b703e6acf9c726942a1e4.sql`
    - `176-38af86134b65d0f10fe33d30dd76442e.sql`
  - _U(15471)_
    - _Rollbacks_
      - `174.sql`
      - `173.sql`
    - `174-bf8229696f7a3bb4700cfddef19fa23f.sql`
    - `173-f7e6c85504ce6e82442c770f7c8606f0.sql`

## Worth noting
1. You can only use the value provided by the global counter as the file name of script files. Any arbitrary file name you set will result into an exception by the utility at the time of release.

2. Only those scripts set as embedded resource are executed at the time of a release. All other scripts which are not an embedded resource needed to be executed manually. This behavior can be used to execute rollback scripts which are not to be executed involuntarily for obvious reasons

3. Never edit a script file once the working branch is moved to any environment given it is testing, UAT or production. In that case, create a new script file and make the changes in it. 

4. The aforementioned file name convention should be followed for both general development & all bug fixing phases. 

5. All scripts should be made idempotent. The state of the database should change to a constant position regardless the number of times a particular script file is executed in that database




## Allowed Namespaces 
| Branch Type |Format |Example  | Remarks|
|--|--|--|--|
| Sprint | S(#sprint_name#) |S(wt1-001)  |
| User Story |  U(#user_story_id#|U(15471)  |
| Task |  T(#task_id#)|  T(15386)|
| Feature |  F(#feature_id#)|F(15478)  |
| Hot Fix |  H(#bug_id#)|H(36589)  | _Use only if a sprint/user story is not available_

## SHJP.Utilities.Counter aka Global Counter

Yet another utility application concerned with script keeping activity. This is a very simple counter mechanism which can provide you a valid filename in sequential order. The counter has the ability to remember the last generated sequence and can generate the next one on demand. 

Since the release pipeline would reject any arbitrary file names at the time of release, it is necessary that you rely upon this counter for getting the script's file name. However you cannot generate not more than one sequence within a span of 24 hours from a single machine. 

[Go to Counter(available only in PITS n\w)](http://counter.spsa.pitsolutions.com/)

>  **Note:** The _EGATEAPI.DataAccess.Deploy_ Utility will execute scripts in alphabetical order of the file name of scripts which are added as an embedded resource. So be keen to generate sequences for scripts, which are depended by other script files before generating the depending scripts

## Generate sequence using visual studio

We have further simplified the sequence generation activity to one more level. Now you can generate the sequence from visual studio itself thus avoiding the need to open global counter in browser.
In order to achieve this functionality in your visual studio, you are required to install [this vsix extension](https://github.com/beingnin/VS-Extensions/releases/download/v1.2/DeployScript.vsix).
#### Installation steps
1. Ensure no visual studio instance is opened
2. Open the .vsix file in visual studio installer application. By default, just double click opening will do the same
3. Do not forget to check the visual studio versions in which you need to have this extensionPress install
4. You will be notified by the installer after a successful installation

#### Usage

The new command to generate script sequence will be available under **Tools --> SHJP --> Generate Script Sequence**. Clicking on it will generate a new sequence from the global counter and the same will be available in your clipboard to be pasted. You can use the clipboard value by pasting it while creating a script file.

> **Note**: All the rules with regard to the usage of global counter from browser is applicable for this extension as well.

