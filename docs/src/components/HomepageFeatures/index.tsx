import type {ReactNode} from 'react';
import clsx from 'clsx';
import Heading from '@theme/Heading';
import styles from './styles.module.css';
import {IconHeadset, IconBroadcast, IconToggleRightFilled} from "@tabler/icons-react";

type FeatureItem = {
  title: string;
  icon: ReactNode;
  description: ReactNode;
};

const FeatureList: { icon: ReactNode; title: string; description: ReactNode }[] = [
    {
        title: '柔軟な送受信システム',
        icon: <IconHeadset width={'48'} height={'48'} />,
        description: (
            <>
                UdonRadioCommunications-Redux は、送受信モジュールを抽象化することで、様々な無線システムに対応できる柔軟性を備えています。複数の送受信モジュールを同時に管理することが可能で、これにより複雑な通信環境を構築することができます。
            </>
        ),
    },
    {
        title: '高度な拡声機能',
        icon: <IconBroadcast width={'48'} height={'48'} />,
        description: (
            <>
                特定の人物に対して、拡声/標準/サイレントの切り替えが可能です。ゾーン情報を利用した拡声状態の自動制御や、ゾーンを跨いでの拡声状態の変更に対応します。また、機内音声によって特定のゾーン内のみ拡声状態を上書きする機能も備えています。
            </>
        ),
    },
    {
        title: '音声 On/Off 機能',
        icon: <IconToggleRightFilled width={'48'} height={'48'} />,
        description: (
            <>
                特定の音声の On/Off を任意に設定することができます。この機能を利用して、例えば ATIS/VOR のような特定の GameObject の On/Off を設定に基づいて制御することが可能です。
            </>
        ),
    },
];

function Feature({title, icon, description}: FeatureItem) {
  return (
    <div className={clsx('col col--4')}>
      <div className="text--center">
          {icon}
      </div>
      <div className="text--center padding-horiz--md">
        <Heading as="h3">{title}</Heading>
        <p>{description}</p>
      </div>
    </div>
  );
}

export default function HomepageFeatures(): ReactNode {
  return (
    <section className={styles.features}>
      <div className="container">
        <div className="row">
          {FeatureList.map((props, idx) => (
            <Feature key={idx} {...props} />
          ))}
        </div>
      </div>
    </section>
  );
}
