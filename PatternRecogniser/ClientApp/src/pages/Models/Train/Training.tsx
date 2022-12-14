import 'antd/dist/antd.min.css';

import { FrownOutlined } from '@ant-design/icons';
import { Row, Typography } from 'antd';

const { Title } = Typography;


const Training = () => {
    return (
        <div>
            <Row align="middle" justify="center">
                <Title>Model jest obecnie trenowany...</Title>
            </Row>
            <Row align="middle" justify="center">
                <Title level={3}>Ten panel jest jeszcze niedokończony.</Title>
            </Row>
            <Row align="middle" justify="center">
                <FrownOutlined style={{ fontSize: '15em', marginTop: "40px" }} spin={true} />
            </Row>
            <Row align="middle" justify="center" style={{ marginTop: "20px" }}>
                <p>a tak naprawdę to nie jest nawet trenowany...</p>
            </Row>
        </div>
    );
}

export default Training;