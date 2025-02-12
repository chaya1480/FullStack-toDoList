require("dotenv").config();
const express = require("express");
const axios = require("axios");
const cors = require("cors");

const app = express();
app.use(cors());

const PORT = process.env.PORT || 3000;
const RENDER_API_KEY = process.env.RENDER_API_KEY;

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

app.listen(PORT, () => {
    console.log(`Server running on port ${PORT}`);
});
