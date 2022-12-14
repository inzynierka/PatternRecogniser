import 'antd/dist/antd.min.css';

import { Loading3QuartersOutlined } from '@ant-design/icons';
import { Row, Typography } from 'antd';

const { Title } = Typography;

export const Loading = () => {
    return (
        <div>
            <Row align="middle" justify="center">
                <Title>Ładowanie...</Title>
            </Row>
            <Row align="middle" justify="center">
                <Loading3QuartersOutlined style={{ fontSize: '15em', marginTop: "40px" }} spin={true} />
            </Row>
        </div>
    )
}