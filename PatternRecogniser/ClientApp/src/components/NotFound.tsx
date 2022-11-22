import 'antd/dist/antd.min.css';

import { Button, Result } from 'antd';
import { Link, useNavigate } from 'react-router-dom';


export default function NotFound() {
    const navigate = useNavigate();

    const NavToHomeHandle = () => {
        navigate('/train', {replace: true});
    }

    return (
        <Result
            status="404"
            title="404"
            subTitle="Przepraszamy, nie znaleźliśmy takiej strony."
            extra={<Button type="primary" onClick={NavToHomeHandle}><Link to="/">Przenieś mnie do <b>Trenuj model</b></Link></Button>}
        />
    );
}

