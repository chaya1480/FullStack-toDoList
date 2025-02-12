require("dotenv").config();
const express = require("express");
const axios = require("axios");
const cors = require("cors");

const app = express();
app.use(cors());

const PORT = process.env.PORT || 3000;
const RENDER_API_KEY = process.env.RENDER_API_KEY;

app.get("/", (req, res) => {
  res.send("אבא תודההההה, השרת עובד!!!!");
});

app.get("/services", async (req, res) => {
    try {
        const response = await axios.get("https://api.render.com/v1/services", {
            headers: { Authorization: `Bearer ${RENDER_API_KEY}` }
        });
        res.json(response.data);
    } catch (error) {
        console.error("Error fetching services:", error);
        res.status(500).json({ error: "Failed to fetch services" });
    }
});

[
    {
      "cursor": "9HKK0I94GmM4aTJqMWs2YzczZmU0azcw",
      "service": {
        "autoDeploy": "yes",
        "branch": "main",
        "createdAt": "2025-02-05T22:20:56.695238Z",
        "dashboardUrl": "https://dashboard.render.com/web/srv-cuhu8i2j1k6c73fe4k70",
        "id": "srv-cuhu8i2j1k6c73fe4k70",
        "name": "ToDoListServer",
        "notifyOnFail": "default",
        "ownerId": "tea-cuhol0tds78s73f0m570",
        "repo": "https://github.com/chaya1480/FullStack-toDoList",
        "rootDir": "TodoApi",
        "serviceDetails": {
          "buildPlan": "starter",
          "env": "docker",
          "envSpecificDetails": {
            "dockerCommand": "",
            "dockerContext": ".",
            "dockerfilePath": "Dockerfile"
          },
          "healthCheckPath": "",
          "maintenanceMode": {
            "enabled": false,
            "uri": ""
          },
          "numInstances": 1,
          "openPorts": null,
          "plan": "free",
          "previews": {
            "generation": "off"
          },
          "pullRequestPreviewsEnabled": "no",
          "region": "singapore",
          "runtime": "docker",
          "sshAddress": "srv-cuhu8i2j1k6c73fe4k70@ssh.singapore.render.com",
          "url": "https://todolistserver-f8wr.onrender.com"
        },
        "slug": "todolistserver-f8wr",
        "suspended": "not_suspended",
        "suspenders": [],
        "type": "web_service",
        "updatedAt": "2025-02-05T23:34:07.325783Z"
      }
    },
    {
      "cursor": "DvYKXsC-EWZsOWwybmcxczczOGFsZWRn",
      "service": {
        "autoDeploy": "yes",
        "branch": "main",
        "createdAt": "2025-02-05T17:06:47.118626Z",
        "dashboardUrl": "https://dashboard.render.com/static/srv-cuhpl9l2ng1s738aledg",
        "id": "srv-cuhpl9l2ng1s738aledg",
        "name": "ToDoListClient",
        "notifyOnFail": "default",
        "ownerId": "tea-cuhol0tds78s73f0m570",
        "repo": "https://github.com/chaya1480/FullStack-toDoList",
        "rootDir": "ToDo-React",
        "serviceDetails": {
          "buildCommand": "npm run build",
          "buildPlan": "starter",
          "previews": {
            "generation": "off"
          },
          "publishPath": "build",
          "pullRequestPreviewsEnabled": "no",
          "url": "https://fullstack-todolist-rcli.onrender.com"
        },
        "slug": "fullstack-todolist-rcli",
        "suspended": "not_suspended",
        "suspenders": [],
        "type": "static_site",
        "updatedAt": "2025-02-11T00:58:33.644398Z"
      }
    }
  ]

app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
