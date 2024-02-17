const express = require('express');
const app = express();
const port = 3005;
const bodyParser = require('body-parser');
const mongoose = require('mongoose');
const cors = require('cors');

require('dotenv').config({ path: 'secrets.env' });
const config = require('./config');
const SECRET = config.JWT_SECRET;
const MONGODB_URI = config.MONGODB_URI;

const swaggerUi = require('swagger-ui-express');
const YAML = require('yamljs');
const swaggerDocument = YAML.load('./swagger.yaml');

app.use('/ui-swagger', swaggerUi.serve, swaggerUi.setup(swaggerDocument));
app.use(bodyParser.json());

// const corsOptions = {
//   origin: ['http://localhost:11009', 'http://frontend-sua:11009', 'http://studentdocker.informatika.uni-mb.si:11004'],
// };
app.use(cors());


app.listen(port, () => {
    console.log(`Server running on port ${port}`);
});

mongoose.connect(MONGODB_URI, {
    useNewUrlParser: true,
    useUnifiedTopology: true,
})
    .then(() => {
        console.log('Connected to MongoDB Atlas');
    })
    .catch((err) => {
        console.error('Error connecting to MongoDB Atlas:', err.message);
    });

const UserStatistics = mongoose.model('userstatistics', {
    endpoint: String,
    timestamp: { type: Date, default: Date.now },
});

app.get('/', (req, res) => {
    res.send('Statistics microservice!');
});


// GET Last Called Endpoint
app.get('/lastCalledEndpoint', async (req, res) => {
    try {
        const lastCall = await UserStatistics.findOne().sort({ timestamp: -1 });
        res.json(lastCall);
    } catch (err) {
        res.status(500).json({ error: 'Error retrieving last call' });
    }
});

// GET Most Called Endpoint
app.get('/mostCalled', async (req, res) => {
    try {
      const mostCalledEndpoint = await UserStatistics.aggregate([
        { $group: { _id: '$endpoint', count: { $sum: 1 } } },
        { $sort: { count: -1 } },
        { $limit: 1 },
      ]);
      
      const result = {
        mostCalledEndpoint: mostCalledEndpoint[0] || null,
        totalCalls: mostCalledEndpoint.length > 0 ? mostCalledEndpoint[0].count : 0
      };
  
      res.json(result);
    } catch (err) {
      res.status(500).json({ error: 'Error retrieving most called endpoint' });
    }
  });
  
  // GET Calls Per Endpoint
  app.get('/callsPerEndpoint', async (req, res) => {
    try {
      const callsPerEndpoint = await UserStatistics.aggregate([
        { $group: { _id: '$endpoint', count: { $sum: 1 } } },
        { $project: { endpoint: '$_id', count: 1, _id: 0 } },
      ]);
  
      const result = {
        callsPerEndpoint,
        totalEndpoints: callsPerEndpoint.length
      };
  
      res.json(result);
    } catch (err) {
      res.status(500).json({ error: 'Error retrieving calls per endpoint' });
    }
  });
  

