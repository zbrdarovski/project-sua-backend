import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.css';
import Shop from './Shop';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Login from './Login';
import Profile from './Profile';
import Register from './Register';
import Delivery from './Delivery';
import Preview from './Preview';
import CommentsRatings from './CommentsRatings';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
    <React.StrictMode>
        <Router>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/shop" element={<Shop />} />
                <Route path="/preview" element={<Preview />} />
                <Route path="/profile" element={<Profile />} />
                <Route path="/register" element={<Register />} />
                <Route path="/delivery" element={<Delivery />} />
                <Route path="/review" element={<CommentsRatings />} />
            </Routes>
        </Router>
    </React.StrictMode>
);
