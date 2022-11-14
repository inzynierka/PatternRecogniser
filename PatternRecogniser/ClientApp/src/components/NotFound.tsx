import { Link, useNavigate } from 'react-router-dom';
import 'antd/dist/antd.min.css';
import { Result, Button } from 'antd';


export default function NotFound() {
    const navigate = useNavigate();

    const NavToHomeHandle = () => {
        navigate('/books/all', {replace: true});
    }

    return (
        <Result
            status="404"
            title="404"
            subTitle="Sorry, the page you visited does not exist."
            extra={<Button type="primary" onClick={NavToHomeHandle}><Link to="/">Back Home</Link></Button>}
        />
    );
}

