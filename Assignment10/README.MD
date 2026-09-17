# JMeter API Performance Testing Assignment

## Project Details

| Item | Details |
|---|---|
| Assignment | Assignment 10 – Performance Testing |
| Tool | Apache JMeter 5.6.3 |
| Test Type | API Performance Testing |
| APIs | REST Countries API and RESTful API Dev |
| Execution Mode | Non-GUI / CLI |
| Report | JMeter HTML Dashboard |

## Project Description

This project implements API performance testing using Apache JMeter.

The test plan covers CSV-based parameterization, REST Countries GET testing, RESTful API Dev POST/GET testing, dynamic data generation, response ID extraction and correlation, assertions, think time, multiple load scenarios, JTL result generation and JMeter HTML dashboard generation.

## APIs Tested

### REST Countries API

```text
Protocol: HTTPS
Host: api.restcountries.com
Endpoint:
/countries/v5/names.common/${searchTerm}
```

Authentication:
```text
Authorization: Bearer ${RESTCOUNTRIES_API_KEY}
```

Header:
```text
Accept: application/json
```

### RESTful API Dev

Create object:
```text
POST /collections/jmeter-test/objects
```

Get created object:
```text
GET /collections/jmeter-test/objects/${createdObjectId}
```

Required headers:
```text
x-api-key: <API_KEY>
Content-Type: application/json
Accept: application/json
```

## Test Data

```csv
searchTerm,productName,productDescription
Canada,iPhone 15,Apple smartphone
Germany,Samsung Galaxy,Android smartphone
India,MacBook Air,Apple laptop
France,Dell XPS,Laptop computer
```

Variables:
```text
searchTerm
productName
productDescription
```

## Test Flow

```text
CSV Data
   ↓
GET Countries
   ↓
Response Code / Duration / JSON Assertions
   ↓
3 Second Think Time
   ↓
Generate Dynamic Product Name + Timestamp
   ↓
POST Create Object
   ↓
Extract Object ID
   ↓
createdObjectId
   ↓
GET Created Object
   ↓
Response Code / Duration / ID Correlation Assertions
```

## Assertions

### GET Countries
- Response Code = `200`
- Response Time <= `3000 ms`
- `data` exists
- `data.objects` exists
- At least one country object is returned
- `names.common` exists and is non-empty

### POST Create Object
- Response Code = `200`
- Response Time <= `3000 ms`
- JSON response validation
- Object ID extraction

### GET Created Object
- Response Code = `200`
- Response Time <= `3000 ms`
- Returned object ID matches the ID extracted from POST

## Dynamic Data and Correlation

JSR223 PreProcessor:

```groovy
def productName = vars.get("productName")
def timestamp = System.currentTimeMillis().toString()
def dynamicProductName = "${productName}-${timestamp}"

vars.put("requestTimestamp", timestamp)
vars.put("dynamicProductName", dynamicProductName)
```

POST response ID extraction:

```text
Variable: createdObjectId
JMESPath: id
```

The extracted ID is used in:

```text
/collections/jmeter-test/objects/${createdObjectId}
```

## Thread Group Configuration

```text
Threads:
${__P(threads,10)}

Ramp-up:
${__P(rampup,10)}

Loop Count:
${__P(loops,-1)}

Duration:
${__P(duration,60)}
```

## Performance Test Scenarios

| Scenario | Threads | Ramp-up | Duration |
|---|---:|---:|---:|
| Light | 10 | 10 sec | 60 sec |
| Moderate | 50 | 30 sec | 60 sec |
| Heavy | 100 | 60 sec | 60 sec |
| Stress | 150 | 30 sec | 60 sec |
| Spike | 100 | 1 sec | 30 sec |
| Soak | 20 | 20 sec | 300 sec |
| Volume | 50 | 30 sec | 120 sec |

# Running the Tests and Generating HTML Reports

Run all commands from the root of the `Assignment10` folder.

Example:

```powershell
cd "C:\path\to\Assignment10"
```

The JMeter executable is:

```text
C:\apache-jmeter-5.6.3\bin\jmeter.bat
```

The JMX file is:

```text
.\TestPlan\API Performance Test.jmx
```

## Real API Test Commands

### Light

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=10 -Jrampup=10 -Jduration=60 -Jscenario=LIGHT -l ".\Results\Light\Light_Load.jtl" -e -o ".\Reports\Light"
```

### Moderate

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=50 -Jrampup=30 -Jduration=60 -Jscenario=MODERATE -l ".\Results\Moderate\Moderate_Load.jtl" -e -o ".\Reports\Moderate"
```

### Heavy

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=100 -Jrampup=60 -Jduration=60 -Jscenario=HEAVY -l ".\Results\Heavy\Heavy_Load.jtl" -e -o ".\Reports\Heavy"
```

### Stress

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=150 -Jrampup=30 -Jduration=60 -Jscenario=STRESS -l ".\Results\Stress\Stress_Load.jtl" -e -o ".\Reports\Stress"
```

### Spike

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=100 -Jrampup=1 -Jduration=30 -Jscenario=SPIKE -l ".\Results\Spike\Spike_Load.jtl" -e -o ".\Reports\Spike"
```

### Soak

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=20 -Jrampup=20 -Jduration=300 -Jscenario=SOAK -l ".\Results\Soak\Soak_Load.jtl" -e -o ".\Reports\Soak"
```

### Volume

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=50 -Jrampup=30 -Jduration=120 -Jscenario=VOLUME -l ".\Results\Volume\Volume_Load.jtl" -e -o ".\Reports\Volume"
```

## Generated Reports

Each command generates a JTL result file and an HTML dashboard.

```text
Results
├── Light/Light_Load.jtl
├── Moderate/Moderate_Load.jtl
├── Heavy/Heavy_Load.jtl
├── Stress/Stress_Load.jtl
├── Spike/Spike_Load.jtl
├── Soak/Soak_Load.jtl
└── Volume/Volume_Load.jtl

Reports
├── Light/index.html
├── Moderate/index.html
├── Heavy/index.html
├── Stress/index.html
├── Spike/index.html
├── Soak/index.html
└── Volume/index.html
```

Open a report:

```powershell
Start-Process ".\Reports\Light\index.html"
```

Change `Light` to the required scenario.

# Real API Test Results

| Scenario | Samples | Failed | Error % | Average (ms) | Min (ms) | Max (ms) | Median (ms) | P90 (ms) | P95 (ms) | P99 (ms) | Transactions/s | Received (KB/s) | Sent (KB/s) |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| Light | 105 | 9 | 8.57% | 2157.53 | 132 | 21443 | 334 | 1341.20 | 21405.50 | 21442.70 | 1.84 | 6.05 | 0.58 |
| Moderate | 395 | 291 | 73.67% | 2552.22 | 129 | 21541 | 328 | 21360.20 | 21397.20 | 21509.20 | 6.90 | 15.49 | 2.13 |
| Heavy | 513 | 440 | 85.77% | 3318.25 | 128 | 21738 | 329 | 21062.60 | 21402.00 | 21540.86 | 6.68 | 12.00 | 2.09 |
| Stress | 2040 | 1858 | 91.08% | 208.32 | 84 | 1231 | 152 | 355.80 | 454.00 | 734.77 | 35.71 | 51.79 | 10.99 |
| Spike | 800 | 677 | 84.63% | 387.95 | 90 | 3939 | 167 | 804.60 | 1383.00 | 3094.95 | 30.25 | 59.76 | 9.58 |
| Soak | 1762 | 1381 | 78.38% | 278.16 | 127 | 1100 | 222 | 425.00 | 782.40 | 966.74 | 5.93 | 13.54 | 1.82 |
| Volume | 1593 | 1491 | 93.60% | 258.66 | 91 | 1411 | 208 | 415.00 | 563.60 | 974.08 | 13.60 | 17.51 | 4.19 |

# Performance Analysis

The real API execution produced failures across all seven scenarios.

- Light: 105 samples, 9 failures, 8.57% error rate.
- Moderate: 395 samples, 291 failures, 73.67% error rate.
- Heavy: 513 samples, 440 failures, 85.77% error rate.
- Stress: 2040 samples, 1858 failures, 91.08% error rate.
- Spike: 800 samples, 677 failures, 84.63% error rate.
- Soak: 1762 samples, 1381 failures, 78.38% error rate.
- Volume: 1593 samples, 1491 failures, 93.60% error rate.

The highest average response time was recorded in the Heavy scenario at 3318.25 ms.

Light, Moderate and Heavy showed maximum response times of approximately 21 seconds.

The percentile metrics also show higher tail response times. For example, Volume had a median of 208 ms and a P99 of 974.08 ms.

The results should be interpreted using response time, percentiles, throughput and error rate together rather than average response time alone.

# Graphs Reviewed

The HTML reports can be reviewed for:

- Response Time Over Time
- Response Time Percentiles
- Response Time Distribution
- Active Threads Over Time
- Transactions per Second
- Hits per Second
- Response Codes per Second
- Latency Over Time
- Connect Time Over Time
- Network Throughput

# Project Structure

```text
Assignment10
│
├── TestPlan
│   └── API Performance Test.jmx
│
├── TestData
│   └── TestData.csv
│
├── Results
│   ├── Light
│   ├── Moderate
│   ├── Heavy
│   ├── Stress
│   ├── Spike
│   ├── Soak
│   └── Volume
│
├── Reports
│   ├── Light
│   ├── Moderate
│   ├── Heavy
│   ├── Stress
│   ├── Spike
│   ├── Soak
│   └── Volume
│
├── Recorder
│   └── HTTPS_Recorder_Demonstration.jmx
│
├── MockApiServer
│   ├── MockApiServer.java
│   └── MockApiServer.class
│
├── Performance_Test_Report.md
└── README.md
```

# Technologies and Concepts Used

- Apache JMeter 5.6.3
- REST API testing
- HTTP/HTTPS
- CSV Data Set Config
- Thread Groups
- HTTP Request Samplers
- Response Assertions
- JSR223 Assertion
- JSR223 PreProcessor
- JMESPath Extractor
- API correlation
- Dynamic test data
- Timers / Think Time
- CLI execution
- JTL result files
- JMeter HTML Dashboard
- Performance metrics and percentile analysis
- Load, Stress, Spike, Soak and Volume testing

# Conclusion

The Assignment 10 JMeter test plan was implemented with parameterized API requests, dynamic data, assertions, response correlation and multiple performance-test scenarios.

The real API execution results are documented above, and the corresponding JMeter HTML dashboards are generated under the `Reports` directory.

# Syllabus Topics Covered

The following Performance Testing and JMeter syllabus topics are covered by this assignment.

## 1. Introduction to Performance Testing

### What is Performance Testing?
Performance testing evaluates an application's behavior, responsiveness, stability and resource usage under a defined workload.

### Need for Performance Testing
The assignment measures response time, latency, throughput, error rate and percentile performance under different workloads.

### Performance Testing in the SDLC
Performance considerations are incorporated during testing rather than being treated only as a final-stage activity. The JMeter test plan provides repeatable performance validation.

### Types of Performance Testing Covered

| Type | Assignment Scenario |
|---|---|
| Load Testing | Light / Moderate / Heavy |
| Stress Testing | Stress |
| Spike Testing | Spike |
| Soak / Endurance Testing | Soak |
| Volume Testing | Volume |

---

## 2. Performance Testing Tools

Apache JMeter is used as the primary performance-testing tool for this assignment.

The assignment demonstrates:
- Test-plan creation
- Thread-based virtual users
- HTTP/API load generation
- Assertions
- Parameterization
- Correlation
- Result collection
- CLI execution
- HTML reporting

---

## 3. Apache JMeter Overview and Test Plan Structure

The project uses an Apache JMeter `.jmx` test plan containing:

- Test Plan
- Thread Group
- HTTP Request Samplers
- Controllers
- Config Elements
- PreProcessors
- PostProcessors
- Assertions
- Timers
- Listeners

---

## 4. Thread Groups

The Thread Group represents virtual users and controls:

- Number of threads/users
- Ramp-up period
- Loop count
- Test duration
- Startup delay

The assignment parameterizes these values using JMeter properties:

```text
${__P(threads,10)}
${__P(rampup,10)}
${__P(loops,-1)}
${__P(duration,60)}
```

This allows one JMX file to execute multiple performance scenarios.

---

## 5. Samplers

HTTP Request samplers are used to test REST APIs.

Implemented requests:

```text
GET Countries
POST Create Object
GET Created Object
```

The requests demonstrate API performance testing using HTTP/HTTPS.

---

## 6. Controllers

The test plan uses controllers to organize the API transaction flow.

The API transaction is structured as:

```text
API Requests
    ↓
Complete API Transaction
    ├── GET Countries
    ├── Think Time
    ├── POST Create Object
    └── GET Created Object
```

---

## 7. Listeners

The test plan uses JMeter listeners for functional verification and result analysis.

Implemented listeners include:

- View Results Tree
- View Results in Table
- Summary Report

For performance execution, results are saved to JTL files and converted into HTML dashboards.

---

## 8. Load Configuration

The assignment demonstrates the major load parameters:

### Number of Users

Controls the number of concurrent virtual users.

### Ramp-up

Controls how quickly JMeter starts the configured users.

### Duration

Controls how long the test remains active.

### Loop Count

Controls iteration behavior when the test is executed without a fixed duration.

---

## 9. HTTP Request Testing

The assignment uses HTTP Request samplers for:

### REST Countries

```text
GET /countries/v5/names.common/${searchTerm}
```

### RESTful API Dev

```text
POST /collections/jmeter-test/objects
GET /collections/jmeter-test/objects/${createdObjectId}
```

---

## 10. Assertions

Assertions are used to validate functional correctness while measuring performance.

Implemented validations include:

- HTTP response code
- Response duration
- JSON response structure
- Country response content
- Created object response
- Correlated object ID

---

## 11. Timers and Think Time

A 3-second Constant Timer is used between the country GET request and the object POST request.

```text
GET Countries
      ↓
3 second Think Time
      ↓
POST Create Object
```

This simulates a pause between user actions instead of sending requests continuously.

---

## 12. Parameterization

CSV Data Set Config is used to avoid hardcoded test data.

Example:

```csv
searchTerm,productName,productDescription
Canada,iPhone 15,Apple smartphone
Germany,Samsung Galaxy,Android smartphone
India,MacBook Air,Apple laptop
France,Dell XPS,Laptop computer
```

JMeter variables are then used in requests:

```text
${searchTerm}
${productName}
${productDescription}
```

---

## 13. JMeter Variables and Properties

JMeter variables are used for data available during a test execution:

```text
${searchTerm}
${productName}
${productDescription}
${createdObjectId}
```

JMeter properties are used to parameterize execution from the command line:

```text
-Jthreads
-Jrampup
-Jduration
-Jscenario
```

---

## 14. PreProcessors

A JSR223 PreProcessor is used before the POST request to generate dynamic test data.

```groovy
def productName = vars.get("productName")
def timestamp = System.currentTimeMillis().toString()
def dynamicProductName = "${productName}-${timestamp}"

vars.put("requestTimestamp", timestamp)
vars.put("dynamicProductName", dynamicProductName)
```

This demonstrates preprocessing and dynamic request-data generation.

---

## 15. PostProcessors

A JMESPath Extractor is used after the POST response to extract the generated object ID.

```text
Variable:
createdObjectId

JMESPath:
id
```

The extracted value is used by the next request.

---

## 16. Correlation

The assignment demonstrates API correlation:

```text
POST Create Object
        ↓
Extract id
        ↓
createdObjectId
        ↓
GET /collections/jmeter-test/objects/${createdObjectId}
```

This ensures that the GET request operates on the object created by the preceding POST request.

---

## 17. Dynamic Data

Dynamic product names and timestamps are generated for POST requests:

```text
productName + timestamp
```

This prevents every virtual user from repeatedly sending exactly the same product name.

---

## 18. API Authentication and Headers

The real API configuration demonstrates authenticated API performance testing.

REST Countries:

```text
Authorization: Bearer ${RESTCOUNTRIES_API_KEY}
Accept: application/json
```

RESTful API Dev:

```text
x-api-key: <API_KEY>
Content-Type: application/json
Accept: application/json
```

---

## 19. Performance Metrics

The generated JMeter reports capture:

- Samples
- Failed requests
- Error percentage
- Average response time
- Minimum response time
- Maximum response time
- Median
- P90
- P95
- P99
- Transactions per second
- Received throughput
- Sent throughput

### Response Time

Time taken for a request to complete.

### Latency

Time until the response begins to arrive.

### Connect Time

Time required to establish the connection.

### Throughput

Amount of processing/request activity completed per unit of time.

### Percentiles

- P90: 90% of recorded requests are at or below the reported response time.
- P95: 95% are at or below the reported response time.
- P99: 99% are at or below the reported response time.

---

## 20. Response Time Analysis

The assignment analyzes:

- Average response time
- Median response time
- Minimum response time
- Maximum response time
- P90
- P95
- P99
- Response-time trends

The HTML dashboards provide graphical response-time analysis.

---

## 21. Throughput Analysis

Transactions per second and network throughput are captured in the JMeter reports.

These metrics are compared across the different workload scenarios.

---

## 22. Error Analysis

The test results include:

```text
#Samples
FAIL
Error %
```

The real API results are recorded in the Results table above, allowing failure rates to be compared across Light, Moderate, Heavy, Stress, Spike, Soak and Volume scenarios.

---

## 23. HTML Performance Reports

JMeter is executed in non-GUI mode with:

```text
-n
```

Results are saved to JTL files using:

```text
-l
```

HTML dashboards are generated using:

```text
-e -o
```

Example:

```powershell
& "C:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t ".\TestPlan\API Performance Test.jmx" -Jthreads=10 -Jrampup=10 -Jduration=60 -Jscenario=LIGHT -l ".\Results\Light\Light_Load.jtl" -e -o ".\Reports\Light"
```

---

## 24. CLI Test Execution

All seven performance scenarios can be executed without the JMeter GUI.

The same `.jmx` test plan is reused with different command-line properties.

Covered scenarios:

```text
Light
Moderate
Heavy
Stress
Spike
Soak
Volume
```

---

## 25. Performance Graph Analysis

The generated HTML dashboards provide graphs used for analysis, including:

- Response Time Over Time
- Response Time Percentiles
- Response Time Distribution
- Active Threads Over Time
- Transactions per Second
- Hits per Second
- Response Codes per Second
- Latency Over Time
- Connect Time Over Time
- Network Throughput

These graphs help identify changes in response time, throughput, concurrency, latency, connection time and HTTP response-code behavior during the tests.

---

## 26. HTTPS Test Script Recorder

A separate recorder demonstration is included:

```text
Recorder
└── HTTPS_Recorder_Demonstration.jmx
```

The recorder configuration demonstrates:

- HTTP(S) Test Script Recorder
- Recording Thread Group
- Recording Controller
- Port `8888`
- Recording into the Recording Controller
- HTTP header capture

---

## 27. Resource and System Observation

During performance execution, system-level CPU and memory observations can be recorded using Windows Task Manager.

These observations are kept separate from JMeter application metrics because they represent the test machine's resource utilization.

---

## 28. Complete Topic Coverage Summary

| Syllabus Topic | Covered |
|---|---|
| Performance Testing Introduction | Yes |
| Need for Performance Testing | Yes |
| Performance Testing in SDLC | Yes |
| Load Testing | Yes |
| Stress Testing | Yes |
| Spike Testing | Yes |
| Soak Testing | Yes |
| Volume Testing | Yes |
| JMeter Overview | Yes |
| JMeter Setup | Yes |
| Test Plan | Yes |
| Thread Groups | Yes |
| Samplers | Yes |
| Listeners | Yes |
| Controllers | Yes |
| Throughput | Yes |
| Response Time | Yes |
| Latency | Yes |
| Connect Time | Yes |
| Average | Yes |
| Median | Yes |
| P90 | Yes |
| P95 | Yes |
| P99 | Yes |
| PreProcessors | Yes |
| PostProcessors | Yes |
| CSV Data Set Config | Yes |
| JMeter Variables | Yes |
| Parameterization | Yes |
| API Correlation | Yes |
| Dynamic Data | Yes |
| Response Assertions | Yes |
| Timers / Think Time | Yes |
| HTTPS Test Script Recorder | Yes |
| Load Users | Yes |
| Ramp-up | Yes |
| Duration | Yes |
| Loop Count | Yes |
| CLI `.jmx` Execution | Yes |
| JTL Results | Yes |
| HTML Reports | Yes |
| Performance Graph Analysis | Yes |
| CPU / Memory Observation | Yes |

---

# Conclusion

The Assignment 10 JMeter test plan was implemented with parameterized API requests, dynamic data, assertions, response correlation and multiple performance-test scenarios.

The real API execution results are documented above, and the corresponding JMeter HTML dashboards are generated under the `Reports` directory.
