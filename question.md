# Answer to Q2:
## We have multiple approaches around multiple types of problems and exceptions.
## Sometimes exceptions are in-service exceptions so we can detect them with:

inline logger
interceptors
metrics
log collectors

## But sometimes exceptions are across our services in a platform so we have to track and trace those exceptions and issues via:
tracking requests alongside each service inline logs
having a log collector service to track each request with its id across a flow between services.

# I implemented multiways in the task and can talk more and complete about how we can detect issues across our platform
