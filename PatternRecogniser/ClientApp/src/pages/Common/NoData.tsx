import 'antd/dist/antd.min.css';

import { InboxOutlined } from '@ant-design/icons';
import { Row, Typography } from 'antd';

const { Title } = Typography;

export const NoData = () => {
    return (
        <div>
            <Row align="middle" justify="center">
                <InboxOutlined style={{ fontSize: '10em', marginTop: "5px", color: "rgb(140, 140, 140)" }}/>
            </Row>
            <Row align="middle" justify="center">
                <Title level={3} type="secondary">Brak danych</Title>
            </Row>
        </div>
    )
}