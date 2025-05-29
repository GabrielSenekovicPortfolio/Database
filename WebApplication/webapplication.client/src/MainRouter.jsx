import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import App from './App';
import SkuPage from './SkuPage';

function MainRouter() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<App />} />
                <Route path="/sku/:sku" element={<SkuPage />} />
            </Routes>
        </Router>
    );
}

export default MainRouter;