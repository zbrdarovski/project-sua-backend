import React from 'react';
import { Link } from 'react-router-dom';
import './Dashboard.css';

function Dashboard() {
    return (
        <div className="dashboard">
            <Link to="/shop">
                <button>Shop</button>
            </Link>
            <Link to="/profile">
                <button>Profile</button>
            </Link>
            <Link to="/">
                <button>Logout</button>
            </Link>
        </div>
    );
}

export default Dashboard;
