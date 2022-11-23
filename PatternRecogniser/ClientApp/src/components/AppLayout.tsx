import '.././custom.css';
import 'antd/dist/antd.min.css';

import { Layout } from 'antd';
import { Component } from 'react';

import { NavMenu } from './NavMenu';

const { Content, Footer } = Layout;

export class AppLayout extends Component {
  static displayName = AppLayout.name;

  render () {
    return (
      <Layout className="layout" style={{ display: 'flex', flexDirection: 'column', flex: 1 }}>
        <NavMenu />
        <Content style={{ padding: '0 50px', flex: 1, minHeight: "81vh"}}>
          {this.props.children}
        </Content>
        <Footer style={{ textAlign: 'center' }}>
          Ant Design ©2018 Created by Ant UED <br /> <br />
          Aplikacja opracowana w ramach pracy inżynierskiej.
        </Footer>
      </Layout>
    );
  }
}
