# Code Demo

## Check and run docker locally

- Docker Version Check <br>
  `docker -v | --version `

- Docker pull Image of redis  <br> ` docker pull redis ` && ` docker image ls ` 

- Run Redis locally <br>
  - Docker RUN ` docker run -p 6379:6379 --name my-local-redis -d redis `
  - Docker containers List ` docker ps `


- Connect to your local redis with redis-cli <br>
  ` docker exec -it my-local-redis redis-cli ` <br>
  ` set mykey1 "WOW! this is working" ` <br>
  ` get mykey1 ` <br>



## Demo Walkthrough

` md docker101 `
` cd docker101 `

` dotnet new console -f net8.0 --use-program-main `

### Add StackExchange.Redis package from nuget

` dotnet add package StackExchange.Redis --version 2.8.0 `


### code to run from outside of docker 

```
using System;
using StackExchange.Redis;
```

```
var con = ConnectionMultiplexer.Connect("localhost:6379");
var db = con.GetDatabase();
var value = db.StringGet("mykey1");
Console.WriteLine($"redis mykey : {value}");
con.Dispose();

```


### Add docker Image 

```

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

USER app
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG configuration=Release
WORKDIR /src
COPY ["docker101/docker101.csproj", "docker101/"]
RUN dotnet restore "docker101/docker101.csproj"
COPY . .
WORKDIR "/src/docker101"
RUN dotnet build "docker101.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "docker101.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "docker101.dll"]


```

or press `ctrl` + `shift` + `P` then search and select `Docker Images: Build Image... `

### take out your docker file from project to workspace folder 
` move dockerfile ..\dockerfile ` && ` cd ..\ `

### Build Docker Image
`docker build . -t 1.0 --no-cache`

### Push Image to hub
`docker tag docker101 rahulmore7861/docker101` <br>
`docker push rahulmore7861/docker101`

## changes to run application and redis from  docker-compose file

- Create new file of docker compose as `docker-compose.yml`  | `New-Item docker-compose.yml`
  paste below mention code into that file

```
services:
  services:"
  docker101-app:
    image: docker101
    command: ["redisdb:6379"]
    build:
      context: .
      dockerfile: ./Dockerfile
    networks:
      - local-net
    depends_on:
      - redisdb 
  redisdb:
    image: redis:latest
    ports:
      - '6379:6379'
    expose:
      - 6379
    networks:
      - local-net
networks: 
  local-net:

```
- change application program like 
```
   var con = ConnectionMultiplexer.Connect("redisdb:6379");

  var db = con.GetDatabase();
  db.StringSet("mykey2", new RedisValue("sample value from c# code"));
  Console.WriteLine("value set;");

  var value = db.StringGet("mykey1");
  var value2 = db.StringGet("mykey2");

  Console.WriteLine($"redis mykey1 : {value}");
  Console.WriteLine($"redis mykey2 : {value2}");
  con.Dispose();
```


### you can also connect to your local-redis through GUI docker hub

` docker exec -it my-local-redis redis-cli `