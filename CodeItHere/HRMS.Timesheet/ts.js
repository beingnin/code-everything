function getDates(range) {
    let re = /(\d{1,2}\/\d{1,2}\/\d{4}-\d{1,2}\/\d{1,2}\/\d{4})|\d{1,2}\/\d{1,2}\/\d{4}/g;
    let m;
    let matches = [];
    let dates = [];

    do {
        m = re.exec(range);
        if (m) {
            m[0] && matches.push(m[0]);
        }
    } while (m);

    for (let match of matches) {
        let splits = match.split('-');
        if (splits.length == 1) {

            dates.push(makeDate(splits[0]))
        }
        else {

            let start = makeDate(splits[0]);
            let current = new Date(start);
            let end = makeDate(splits[1]);

            start.getTime() < end.getTime() && dates.push(start);

            while (current.getTime() < end.getTime()) {
                current = new Date(current.getFullYear(), current.getMonth(), current.getDate() + 1);
                dates.push(current);
            }
        }
    }

    return dates;
}
function makeDate(dd_MM_YYY) {
    let splits = dd_MM_YYY.split('/');
    return new Date(`${splits[1]}/${splits[0]}/${splits[2]}`);
}

let maketwodigit = (num) => num.toString().length == 1 ? '0' + num.toString() : num.toString();

async function ts(sprint, range, employeeId = 17960) {



    let dates = getDates(range);
    let failed = [];

    for (let date of dates) {

        date = `${date.getFullYear()}-${maketwodigit(date.getMonth() + 1)}-${maketwodigit(date.getDate())}`




        await fetch("https://hrms.pitsolutions.com:8091/api/TimeSheet/AddTimesheet", {
            "headers": {
                "accept": "*/*",
                "accept-language": "en-IN,en-GB;q=0.9,en-US;q=0.8,en;q=0.7",
                "authorization": "Bearer c1M0c3VmYU11aThNalZpZGNJN2lGRjI4UUt3SzVrUCtvNG9IWjQzUmlTRT06UElUMTAzNDo6NjM3NzE1MjI4NDM2Mzg3NDkzOm5hbWU6OTYzOA==",
                "content-type": "application/json;charset=UTF-8",
                "sec-ch-ua": "\"Google Chrome\";v=\"95\", \"Chromium\";v=\"95\", \";Not A Brand\";v=\"99\"",
                "sec-ch-ua-mobile": "?0",
                "sec-ch-ua-platform": "\"Windows\"",
                "sec-fetch-dest": "empty",
                "sec-fetch-mode": "cors",
                "sec-fetch-site": "same-site"
            },
            "referrer": "https://hrms.pitsolutions.com/",
            "referrerPolicy": "strict-origin-when-cross-origin",
            "body": "{\"employeeId\":" + employeeId + ",\"timeSheetDate\":\"" + date + "\",\"timesheetStatus\":\"PENDING\",\"timeSheetSubmit\":false,\"timesheetMaxLogHoursPerDay\":24,\"tasks\":[{\"title\":\"Technical support\",\"taskid\":null,\"taskDescription\":\"Support to team members\",\"entryUser\":9638,\"taskTimeTaken\":\"08:00\",\"sourceId\":0,\"timesheetDetailId\":0,\"timesheetId\":0,\"projectId\":192,\"Project\":\"SPSA\",\"subProject\":\"SPSA\\\\MNT-" + sprint + "\"}]}",
            "method": "POST",
            "mode": "cors",
            "credentials": "include"
        }).then((response) => response.json())
            .then(function (evp) {
                console.log(evp);
            })
            .catch(function (err) {
                console.log(err);
                failed.push(date)

            })
        console.info(`Logged for ${date}`);
    }


    console.log(failed)


}


