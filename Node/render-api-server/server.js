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

// app.get("/services", async (req, res) => {
//   try {
//     const response = await axios.get("https://api.render.com/v1/services", {
//       headers: { Authorization: `Bearer ${RENDER_API_KEY}` },
//     });
//     res.json(response.data);
//   } catch (error) {
//     console.error("Error fetching services:", error);
//     res.status(500).json({ error: "Failed to fetch services" });
//   }
// });
app.get("/services", async (req, res) => {
  try {
      const response = await axios.get("https://api.render.com/v1/services", {
          headers: { Authorization: `Bearer ${RENDER_API_KEY}` }
      });

      const services = response.data;

      let html = `
          <html>
          <head>
              <title>Services List</title>
              <style>
                  body { font-family: Arial, sans-serif; margin: 20px; }
                  table { width: 100%; border-collapse: collapse; margin-top: 20px; }
                  th, td { border: 1px solid black; padding: 10px; text-align: left; }
                  th { background-color: #f2f2f2; }
              </style>
          </head>
          <body>
              <h1>List of Services</h1>
              <table>
                  <tr>
                      <th>Name</th>
                      <th>Type</th>
                      <th>URL</th>
                      <th>Created At</th>
                  </tr>`;

      services.forEach(service => {
          html += `
              <tr>
                  <td>${service.service.name}</td>
                  <td>${service.service.type}</td>
                  <td><a href="${service.service.serviceDetails?.url}" target="_blank">${service.service.serviceDetails?.url || "N/A"}</a></td>
                  <td>${new Date(service.service.createdAt).toLocaleString()}</td>
              </tr>`;
      });

      html += `
              </table>
          </body>
          </html>`;

      res.send(html);

  } catch (error) {
      console.error("Error fetching services:", error);
      res.status(500).send("<h1>Error fetching services</h1>");
  }
});

app.listen(PORT, () => {
  console.log(`Server running on port ${PORT}`);
});
