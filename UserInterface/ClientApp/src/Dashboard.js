import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Dashboard.css';

function Dashboard() {
    const navigate = useNavigate();

    const handleLogout = () => {
        // Clear relevant items from localStorage
        localStorage.removeItem('token');
        localStorage.removeItem('userId');

        // Redirect to the login page or another appropriate route
        navigate('/');
    };

    return (
        <div className="dashboard">
            <Link to="/shop">
                <button>Shop</button>
            </Link>
            <Link to="/preview">
                <button>Preview</button>
            </Link>
            <Link to="/profile">
                <button>Profile</button>
            </Link>
            <Link to="/review">
                <button>Review</button>
            </Link>
            <button onClick={handleLogout}>Logout</button>
        </div>
    );
}

export default Dashboard;
