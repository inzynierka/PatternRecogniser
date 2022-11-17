import {Typography, Card, Space, Tooltip, Input, Button } from "antd"

import 'antd/dist/antd.min.css';
import { Row, Col } from "antd";
import { useState } from "react";
import { QuestionCircleOutlined, SearchOutlined } from '@ant-design/icons';
import { useNavigate } from 'react-router-dom';

const { Title } = Typography;

const ComparisonPage = () => {

    return (
        <div>
            <Row style={{ marginTop: 50 }}>
                <Col flex="auto">
                    <div className="site-layout-content" style={{minHeight: "75vh" }}>
                        <Row justify="space-around" align="middle" style={{marginBottom: "30px"}}>
                            <Title>Porównywarka</Title>
                        </Row>
                    </div>      
                </Col>
            </Row>
        </div>
    );
}

export default ComparisonPage;