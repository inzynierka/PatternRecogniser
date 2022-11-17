import {Typography, Card, Button } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col } from "antd";
import { DeleteOutlined } from '@ant-design/icons';
import { ModelClass } from "../classes/Model";

const { Title } = Typography;

interface Props {
    model: ModelClass
}

const ModelListElement = (props: Props) => {
    const displaySymbolNumber = (num : number)  => {
        if(num === 1) return num.toString() + " symbol"
        if(num % 10 >= 2 && num % 10 <= 4) return num.toString() + " symbole"
        return num.toString() + " symboli"
    }


    return (
        <div key={"div_" + props.model.name}>
            <Card>
                <Row justify="space-between" align="middle">
                    <Col>
                        <Title level={3}>{props.model.name}</Title>
                        {displaySymbolNumber(props.model.patternNum)}
                    </Col>
                    <Col>
                        <Row justify="end">
                            <Button type="primary" style={{width: "100px", marginBottom: '10px'}} size="large" key={"detailsButton_" + props.model.name}>Szczegóły</Button>
                        </Row>
                        <Row justify="end">
                            <Button type="default" shape="circle" icon={<DeleteOutlined />} key={"deleteButton" + props.model.name} size="large" />
                        </Row>
                    </Col>
                </Row>
                
            </Card>
        </div>
    );
}

export default ModelListElement;